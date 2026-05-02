import { Injectable } from '@angular/core';
import Keycloak from 'keycloak-js';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private keycloak: Keycloak;
  private userPermissions: string[] = [];
  
  // In-memory token storage (XSS protection vs localStorage)
  private _token: string | undefined;
  private _refreshToken: string | undefined;
  private _idToken: string | undefined;
  private _profileLoadPromise: Promise<void> | null = null;

  constructor() {
    this.keycloak = new Keycloak({
      url: environment.keycloak.url,
      realm: environment.keycloak.realm,
      clientId: environment.keycloak.clientId
    });
  }

  get isAuthenticated(): boolean {
    return !!this._token;
  }

  hasPermission(permission: string): boolean {
    if (this.isSuperAdmin) return true;
    if (this.userPermissions.includes('*')) return true;
    if (this.userPermissions.includes(permission)) return true;
    if (permission.endsWith('.View')) {
      const writePerm = permission.replace('.View', '.Write');
      return this.userPermissions.includes(writePerm);
    }
    return false;
  }

  public get isSuperAdmin(): boolean {
    if (!this._token) return false;
    try {
      const base64Url = this._token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const parsed = JSON.parse(window.atob(base64));
      const roles = (parsed?.['realm_access']?.['roles'] || []) as string[];
      return roles.some(r => r.toLowerCase() === 'admin' || r.toLowerCase() === 'framework-admin');
    } catch {
      return false;
    }
  }

  async init(): Promise<boolean> {
    return new Promise((resolve) => {
      const timeout = setTimeout(() => {
        console.warn('Keycloak init timed out after 10s');
        resolve(false);
      }, 10000);

      try {
        // Try checking SSO via Keycloak JS
        this.keycloak.init({
          onLoad: 'check-sso',
          checkLoginIframe: false,
          silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html',
          enableLogging: false
        }).then(async authenticated => {
          clearTimeout(timeout);
          if (authenticated && this.keycloak.token) {
            this._token = this.keycloak.token;
            this._refreshToken = this.keycloak.refreshToken;
            this._idToken = this.keycloak.idToken;
            await this.loadUserProfile();
          }
          resolve(authenticated);
        }).catch(error => {
          clearTimeout(timeout);
          console.warn('Keycloak init failed:', error);
          resolve(false);
        });
      } catch {
        clearTimeout(timeout);
        resolve(false);
      }
    });
  }

  private async loadUserProfile(): Promise<void> {
    if (!this._token) return;
    
    // Prevent race conditions by reusing the same promise if already loading
    if (this._profileLoadPromise) {
      return this._profileLoadPromise;
    }

    this._profileLoadPromise = (async () => {
      try {
        const response = await fetch(`${environment.apiUrl}/api/identity/me`, {
          headers: {
            'Authorization': `Bearer ${this._token}`
          }
        });
        if (response.ok) {
          const result = await response.json();
          this.userPermissions = result.data?.permissions ?? [];
        }
      } catch (e) {
        console.error('Failed to load user profile', e);
      } finally {
        this._profileLoadPromise = null;
      }
    })();

    return this._profileLoadPromise;
  }

  async loginWithCredentials(username: string, password: string): Promise<boolean> {
    const tokenUrl = `/keycloak-auth/realms/${environment.keycloak.realm}/protocol/openid-connect/token`;
    const formBody = new URLSearchParams({
      client_id: environment.keycloak.clientId,
      username,
      password,
      grant_type: 'password',
      scope: 'openid'
    }).toString();

    try {
      const response = await fetch(tokenUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8' },
        body: formBody,
        mode: 'cors'
      });

      if (response.ok) {
        const data = await response.json();
        this._token = data.access_token;
        this._refreshToken = data.refresh_token;
        this._idToken = data.id_token;

        await this.loadUserProfile();
        return true;
      } else {
        const errorData = await response.json().catch(() => ({}));
        console.error('Login failed:', errorData);
        return false;
      }
    } catch (error) {
      console.error('Login failed', error);
      return false;
    }
  }

  async refreshToken(): Promise<boolean> {
    if (!this._refreshToken) {
      this.logout();
      return false;
    }

    const tokenUrl = `/keycloak-auth/realms/${environment.keycloak.realm}/protocol/openid-connect/token`;
    const formBody = new URLSearchParams({
      client_id: environment.keycloak.clientId,
      grant_type: 'refresh_token',
      refresh_token: this._refreshToken
    }).toString();

    try {
      const response = await fetch(tokenUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8' },
        body: formBody,
        mode: 'cors'
      });

      if (response.ok) {
        const data = await response.json();
        this._token = data.access_token;
        if (data.refresh_token) {
           this._refreshToken = data.refresh_token;
        }
        return true;
      } else {
        this.logout();
        return false;
      }
    } catch {
      this.logout();
      return false;
    }
  }

  get token(): string | undefined {
    return this._token;
  }

  get username(): string | undefined {
    if (!this._token) return undefined;
    try {
      const base64Url = this._token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const parsed = JSON.parse(window.atob(base64));
      return parsed?.['preferred_username'];
    } catch {
      return undefined;
    }
  }

  logout() {
    this._token = undefined;
    this._refreshToken = undefined;
    this._idToken = undefined;
    this.userPermissions = [];
    this.keycloak.logout();
  }
}


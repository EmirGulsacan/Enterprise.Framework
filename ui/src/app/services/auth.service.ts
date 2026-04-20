import { Injectable } from '@angular/core';
import Keycloak from 'keycloak-js';
import { environment } from '../../environments/environment';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private keycloak: Keycloak;
  private userPermissions: string[] = [];
  constructor() {
    this.keycloak = new Keycloak({
      url: environment.keycloak.url,
      realm: environment.keycloak.realm,
      clientId: environment.keycloak.clientId
    });
  }
  get isAuthenticated(): boolean {
    return this.keycloak.authenticated ?? false;
  }
  hasPermission(permission: string): boolean {
    if (this.isSuperAdmin) return true;
    return this.userPermissions.includes(permission);
  }
  public get isSuperAdmin(): boolean {
    return this.keycloak.tokenParsed?.['preferred_username'] === 'enterprise_admin';
  }
  async init(): Promise<boolean> {
    return new Promise((resolve) => {
      const timeout = setTimeout(() => {
        console.warn('Keycloak init timed out after 10s');
        resolve(false);
      }, 10000);
      try {
        const storedToken = localStorage.getItem('kc_token');
        const storedRefreshToken = localStorage.getItem('kc_refreshToken');
        const storedIdToken = localStorage.getItem('kc_idToken');

        this.keycloak.init({
          onLoad: 'check-sso',
          checkLoginIframe: false,
          silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html',
          token: storedToken || undefined,
          refreshToken: storedRefreshToken || undefined,
          idToken: storedIdToken || undefined,
          enableLogging: true
        }).then(async authenticated => {
          clearTimeout(timeout);
          if (authenticated) {
            await this.loadUserProfile();
          }
          resolve(authenticated);
        }).catch(error => {
          clearTimeout(timeout);
          console.warn('Keycloak init failed:', error);
          resolve(false);
        });
      } catch (err) {
        clearTimeout(timeout);
        resolve(false);
      }
    });
  }
  private async loadUserProfile(): Promise<void> {
    try {
      const response = await fetch('http://localhost:5200/api/identity/me', {
        headers: {
          'Authorization': `Bearer ${this.keycloak.token}`
        }
      });
      if (response.ok) {
        const result = await response.json();
        this.userPermissions = result.data.permissions || [];
      }
    } catch (e) {
      console.error('Failed to load user profile', e);
    }
  }
  async loginWithCredentials(username: string, password: string): Promise<boolean> {
    const details: any = {
      'client_id': environment.keycloak.clientId,
      'username': username,
      'password': password,
      'grant_type': 'password',
      'scope': 'openid'
    };
    const formBody = Object.keys(details).map(key => encodeURIComponent(key) + '=' + encodeURIComponent(details[key])).join('&');
    try {
      const response = await fetch(`/keycloak-auth/realms/enterprise-realm/protocol/openid-connect/token`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8' },
        body: formBody,
        mode: 'cors'
      });
      if (response.ok) {
        const data = await response.json();
        this.keycloak.token = data.access_token;
        this.keycloak.refreshToken = data.refresh_token;
        this.keycloak.idToken = data.id_token;
        
        localStorage.setItem('kc_token', data.access_token);
        if (data.refresh_token) localStorage.setItem('kc_refreshToken', data.refresh_token);
        if (data.id_token) localStorage.setItem('kc_idToken', data.id_token);

        (this.keycloak as any).authenticated = true;
        if (data.access_token) {
          const base64Url = data.access_token.split('.')[1];
          const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
          (this.keycloak as any).tokenParsed = JSON.parse(window.atob(base64));
        }
        await this.loadUserProfile();
        return true;
      } else {
        const errorData = await response.json().catch(() => ({}));
        console.error('Login failed response:', errorData);
        return false;
      }
    } catch (error) {
      console.error('Direct login failed', error);
      return false;
    }
  }
  async loginDirect(username: string, password: string): Promise<boolean> {
     const details: any = {
        'client_id': environment.keycloak.clientId,
        'username': username,
        'password': password,
        'grant_type': 'password',
        'scope': 'openid'
    };
    const formBody = Object.keys(details).map(key => encodeURIComponent(key) + '=' + encodeURIComponent(details[key])).join('&');
    try {
        const response = await fetch('http://localhost:8080/realms/enterprise-realm/protocol/openid-connect/token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8'
            },
            body: formBody
        });
        if (response.ok) {
            const data = await response.json();
            return true;
        }
        return false;
    } catch (e) {
        return false;
    }
  }
  get token(): string | undefined {
    return this.keycloak.token;
  }
  get username(): string | undefined {
    return this.keycloak.tokenParsed?.['preferred_username'];
  }
  logout() {
    localStorage.removeItem('kc_token');
    localStorage.removeItem('kc_refreshToken');
    localStorage.removeItem('kc_idToken');
    this.keycloak.logout();
  }
}

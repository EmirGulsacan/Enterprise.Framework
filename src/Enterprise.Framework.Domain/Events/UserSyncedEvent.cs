namespace Enterprise.Framework.Domain.Events;

using Enterprise.Framework.Domain.Common;

using Enterprise.Framework.Domain.Entities.Identity;



public class UserSyncedEvent : BaseEvent {

public AppUser User  { get; }UserSyncedEvent(AppUser user) {
    
User = user;

    }
}




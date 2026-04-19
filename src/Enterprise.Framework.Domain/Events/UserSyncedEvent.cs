namespace Enterprise.Framework.Domain.Events;

using Enterprise.Framework.Domain.Common;
using Enterprise.Framework.Domain.Entities;

public class UserSyncedEvent : BaseEvent
{
    public AppUser User { get; }

    public UserSyncedEvent(AppUser user)
    {
        User = user;
    }
}

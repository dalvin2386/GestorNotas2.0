using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GestorNotas2._0.Messages
{
    public class NoteSavedMessage : ValueChangedMessage<bool>
    {
        public NoteSavedMessage(bool value) : base(value)
        {
        }
    }
}

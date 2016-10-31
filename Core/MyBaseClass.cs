namespace Core
{
    public class MyBaseClass : GalaSoft.MvvmLight.ViewModelBase
    {
        protected virtual void Broadcast(object oldValue, object newValue, string propertyName)
        {
            var message = new PropertyChangedMessage(this, this, oldValue, newValue, propertyName);
            if (MessengerInstance != null)
            {
                MessengerInstance.Send(message);
            }
            else
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(message);
            }
        }

        protected virtual void RaisePropertyChanged(string propertyName, object oldValue, object newValue)
        {
            RaisePropertyChanged(propertyName);
            Broadcast(oldValue, newValue, propertyName);
        }
    }
}
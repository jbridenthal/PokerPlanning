

namespace PokerPlanning.Client.Services
{
    public class AppStateService
    {
        public User? CurrentUser { get; set; }

        public event Action OnChange;
        public event Action OnNameChange;
        public event Action OnRoleChange;

        public void SetName(string name)
        {
            if (CurrentUser != null)
            {
                CurrentUser.Name = name;
                OnNameChange.Invoke();
                NotifyStateChanged();
            }
        }

        public void SetUser(User user)
        {
            CurrentUser = user;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

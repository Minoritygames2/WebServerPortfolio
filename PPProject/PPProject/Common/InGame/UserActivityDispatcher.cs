namespace PPProject.Common.InGame
{
    public class UserActivityDispatcher
    {
        private event Func<long, Task>? _userLoggedIn;

        public void AddUserLoggedInListener(Func<long, Task> handler)
        {
            _userLoggedIn += handler;
        }

        public async Task OnUserLoggedIn(long uId)
        {
            if (_userLoggedIn == null)
                return;

            foreach (var handler in _userLoggedIn.GetInvocationList())
            {
                try
                {
                    if (handler == null)
                        continue;
                    await ((Func<long, Task>)handler)(uId);
                }
                catch(Exception e)
                {

                }
            }
        }
    }
}

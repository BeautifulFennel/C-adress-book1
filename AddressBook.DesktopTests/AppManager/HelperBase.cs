namespace addressbook_tests_white
{
    public abstract class HelperBase
    {
        protected readonly ApplicationManager manager;

        protected HelperBase(ApplicationManager manager)
        {
            this.manager = manager;
        }
    }
}

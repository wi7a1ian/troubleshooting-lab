using System;

namespace ex_unhandled_module_managed_dll
{
    public class SomeClass
    {
        public void ProcessSomethingImportant()
        {
            throw new NotImplementedException("Sorry!");
        }
    }
}

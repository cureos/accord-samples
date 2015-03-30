using Cirrious.CrossCore.IoC;

namespace IPPrototyper.Core
{
    public class App : Cirrious.MvvmCross.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            this.CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
                
            RegisterAppStart<ViewModels.MainViewModel>();
        }
    }
}
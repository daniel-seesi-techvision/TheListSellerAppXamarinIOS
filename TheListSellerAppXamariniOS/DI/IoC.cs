using System;
using System.IO;
using TheListSellerAppXamariniOS.Data.Database;
using TheListSellerAppXamariniOS.Data.Repository;
using Unity;
using Unity.Injection;
using static TheListSellerAppXamariniOS.Constants.StringConstants;

namespace TheListSellerAppXamariniOS.DI
{
    public static class IoC
    {
        public static UnityContainer Container { get; set; } = new UnityContainer();
        public static void Init()
        {
            RegisterDbContext();
            Container.RegisterType(typeof(IDataRepository<>), typeof(DataRepository<>));
        }
       
        public static T Get<T>(string name = null)
        {
            return Container.Resolve<T>(name);
        }

        private static void RegisterDbContext()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder instead

            var dbPath = Path.Combine(libraryPath, DATABASE_FILENAME);
            Container.RegisterType<AppDbContext>
                (new InjectionConstructor(new InjectionParameter(dbPath)));
        }
    }
}


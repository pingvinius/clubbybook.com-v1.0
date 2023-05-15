namespace ClubbyBook.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects.DataClasses;
    using ClubbyBook.Business;

    public static class ControllerFactory
    {
        private static Dictionary<Type, object> controllersCache = new Dictionary<Type, object>();
        private static object syncObject = new object();

        public static EntityControllerType GetInstance<EntityType, EntityControllerType>()
            where EntityType : EntityObject
            where EntityControllerType : IEntityController<EntityType>
        {
            Type entityControllerType = typeof(EntityControllerType);

            if (!controllersCache.ContainsKey(entityControllerType))
            {
                lock (syncObject)
                {
                    if (!controllersCache.ContainsKey(entityControllerType))
                    {
                        controllersCache.Add(entityControllerType, Activator.CreateInstance<EntityControllerType>());
                    }
                }
            }

            return (EntityControllerType)controllersCache[entityControllerType];
        }

        public static AuthorsController AuthorsController
        {
            get
            {
                return ControllerFactory.GetInstance<Author, AuthorsController>();
            }
        }

        public static BooksController BooksController
        {
            get
            {
                return ControllerFactory.GetInstance<Book, BooksController>();
            }
        }

        public static NewsController NewsController
        {
            get
            {
                return ControllerFactory.GetInstance<News, NewsController>();
            }
        }

        public static ProfilesController ProfilesController
        {
            get
            {
                return ControllerFactory.GetInstance<Profile, ProfilesController>();
            }
        }

        public static NotificationsController NotificationsController
        {
            get
            {
                return ControllerFactory.GetInstance<Notification, NotificationsController>();
            }
        }

        public static GenresController GenresController
        {
            get
            {
                return ControllerFactory.GetInstance<Genre, GenresController>();
            }
        }

        public static CitiesController CitiesController
        {
            get
            {
                return ControllerFactory.GetInstance<City, CitiesController>();
            }
        }

        public static UsersController UsersController
        {
            get
            {
                return ControllerFactory.GetInstance<User, UsersController>();
            }
        }
    }
}
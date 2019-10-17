using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac.Integration.Mvc;
using System.Reflection;
using WorkV3.Models.Repository;
using WorkV3.Models.Interface;
using System.Web.Mvc;
using System.Web.Http;
using WorkV3.Models;
using WorkV3.Models.Service;

namespace WorkV3.App_Start
{
    public class AutofacConfig
    {
        public static void Bootstrapper()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType(typeof(MemberShipRepository)).As(typeof(IMemberShipRepository<MemberShipModels>));
            builder.RegisterType(typeof(PageRepository)).As(typeof(IPageRepository<PagesModels>));
            builder.RegisterType(typeof(EmailService)).As(typeof(IEmailService));
            builder.Register(c => new PointRepository(c.Resolve<IMemberShipRepository<MemberShipModels>>())).As(typeof(IPointRepository<float>));

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
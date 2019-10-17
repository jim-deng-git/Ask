using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkV3.Areas.Backend.ViewModels.CustomCompany;
using WorkV3.Common;
using WorkV3.Models;
using WorkV3.Models.Interface;
using WorkV3.Models.Repository;

namespace WorkV3.Areas.Backend.Controllers
{
    public class CustomCompanyController : Controller
    {
        private ICustomCompanyRepository<CustomCompanyModel> customRepository = new CustomCompanyRepository();
        private ICustomCompanyContactRepository<CustomCompanyContactModel> contactRepository = new CustomCompanyContactRepository();

        public ActionResult List(long siteId, long backMenu, int index = 1)
        {
            var pagination = new Pagination
            {
                PageIndex = index,
                PageSize = WebInfo.PageSize
            };

            var model = new CustomCompanyListViewModel();

            int recordCount;
            model.Companies = customRepository.GetItems(pagination.PageSize, pagination.PageIndex, out recordCount, siteId, "SiteID", " Sort, CreateTime Desc ");

            pagination.TotalRecord = recordCount;
            model.Pagination = pagination;

            model.SiteID = siteId;
            model.MenuID = backMenu;

            return View(model);
        }

        public ActionResult Add(long siteId, bool isExit = false)
        {
            var model = new CustomCompanyAddViewModel();

            model.ID = WorkLib.GetItem.NewSN();
            model.SiteID = siteId;

            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CustomCompanyModel model, List<CustomCompanyContactModel> contacts)
        {
            model.ModifyTime = model.CreateTime;
            model.Modifier = Models.DataAccess.MemberDAO.SysCurrent.Id;

            customRepository.CreateItem(model);

            int idx = 1;
            foreach(var contact in contacts)
            {
                contact.ID = WorkLib.GetItem.NewSN();
                contact.CustomCompanyID = model.ID;
                contact.Sort = idx;
                contactRepository.CreateItem(contact);
            }

            return RedirectToAction("Edit", new { id = model.ID, isExit = true });
        }

        public ActionResult Edit(long id, bool isExit = false)
        {
            var model = new CustomCompanyEditViewModel();
            model.IsExit = isExit;
            model.Company = customRepository.GetItem(id);
            model.ModifierName = Models.DataAccess.MemberDAO.SysCurrent.Name;
            model.Contacts = contactRepository.GetItems(id, "CustomCompanyID");

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CustomCompanyModel model, List<CustomCompanyContactModel> contacts)
        {
            var currentData = customRepository.GetItem(model.ID);

            currentData.Name = model.Name;
            currentData.TaxIdNumber = model.TaxIdNumber;
            currentData.AccountCloseDateStart = model.AccountCloseDateStart;
            currentData.AccountCloseDateEnd = model.AccountCloseDateEnd;
            currentData.PayingPeriod = model.PayingPeriod;
            currentData.PayDescription = model.PayDescription;
            currentData.Address = model.Address;
            currentData.ModifyTime = DateTime.Now;
            currentData.Modifier = Models.DataAccess.MemberDAO.SysCurrent.Id;

            customRepository.UpdateItem(currentData, new string[] { "Name", "TaxIdNumber", "AccountCloseDateStart", "AccountCloseDateEnd", "PayingPeriod", "PayDescription", "Address", "ModifyTime", "Modifier" });

            foreach(var contact in contacts)
            {
                if(contact.ID == 0)
                {
                    contact.ID = WorkLib.GetItem.NewSN();
                    contact.CustomCompanyID = model.ID;
                    contactRepository.CreateItem(contact);
                }
                else
                {
                    contactRepository.UpdateItem(contact, new string[] { "Name", "Email", "Phone" });
                }
            }

            return RedirectToAction("Edit", new { id = model.ID, isExit = true });
        }

        [HttpPost]
        public ActionResult Sort(List<SortItem> items)
        {
            try
            {
                customRepository.Sort(items);
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(List<long> ids)
        {
            try
            {
                customRepository.Delete(ids);
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteContacts(List<long> ids)
        {
            try
            {
                contactRepository.Delete(ids);
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
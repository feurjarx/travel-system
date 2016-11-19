using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlTravelAgencySystem.Models;
using ControlTravelAgencySystem.Common;

namespace ControlTravelAgencySystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly TravelSystemEntities _dbContext;
        public EmployeeController(TravelSystemEntities dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Создание нового сотрудника (вкладка Сотрудники Админка)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Create()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                // создаем объект класса - Пдн
                person employeePerson = new person();
                serializePersonToModal(ref employeePerson);
                _dbContext.people.Add(employeePerson);

                // создаем объект класса - Сотрудник
                employee employee = new employee();
                employee.person = employeePerson; // присваиваем новые Пдн
                serializeEmployeeToModel(ref employee);

                // Добавляем в коллекцию
                _dbContext.employees.Add(employee);
                // сохранение в базу
                _dbContext.SaveChanges();

            } catch(Exception exc)
            {
                result.Add("error", exc.Message);
            }
            
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult Edit(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                employee employee = _dbContext.employees.Find(id);
                serializeEmployeeToModel(ref employee);

                person employeePerson = employee.person;
                serializePersonToModal(ref employeePerson);
                
                // сохранение в базу
                _dbContext.SaveChanges();
            }
            catch (Exception exc)
            {
                result.Add("error", exc.Message);
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        
        
        private void serializeEmployeeToModel(ref employee employee)
        {
            employee.created_at = Utils.dtToTimestamp(DateTime.Now);
            employee.position = Request.Form["position"];
            
            if (Request.Form["salary"] != null && Request.Form["salary"] != "")
            {
                employee.salary = Int32.Parse(Request.Form["salary"]);
            }

            employee.email = Request.Form["email"];
            employee.password = Utils.md5(Request.Form["password"]);
        }

        private void serializePersonToModal(ref person person)
        {
            person.fullname = Request.Form["fullname"];
            person.birthday_at = Utils.dtToTimestamp(Convert.ToDateTime(Request.Form["birthday_at"]));
            person.passport_code = Request.Form["passport_code"] + Request.Form["passport_series"];   
        }
        
        /// <summary>
        /// Процедура увольнения
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public JsonResult Delete(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            
            try
            {
                // получаем сотрудника с базы
                employee employeeFromDb = _dbContext.employees
                    .Where(e => e.id == id)
                    .FirstOrDefault();

                if (employeeFromDb != null)
                {
                    _dbContext.employees.Remove(employeeFromDb);
                    _dbContext.SaveChanges();
                }
                else
                {
                    result.Add("error", "Работник не найден");
                }
            }
            catch(Exception exc)
            {
                result.Add("error", exc.Message);
            }
            
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}
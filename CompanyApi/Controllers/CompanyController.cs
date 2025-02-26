﻿using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> Create(CreateCompanyRequest request)
        {
            if (companies.Exists(company => company.Name.Equals(request.Name)))
            {
                return BadRequest();
            }
            Company companyCreated = new Company(request.Name);
            companies.Add(companyCreated);
            return StatusCode(StatusCodes.Status201Created, companyCreated);
        }

        [HttpDelete]
        public void ClearData()
        {
            companies.Clear();
        }

        [HttpGet]
        public ActionResult<List<Company>> Get()
        {
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public ActionResult<Company> GetById(string id)
        {
            Company? company = companies.FirstOrDefault(c => c.Id.Equals(id));
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }

        [HttpGet("details")]
        public ActionResult<List<Company>> GetXCompaniesFromYPage(int pageSize, int pageIndex)
        {
            int startIndex = (pageIndex-1) * pageSize;
            int endIndex = pageIndex * pageSize > companies.Count ? companies.Count : pageIndex * pageSize;
            if (startIndex >= companies.Count)
            {
                return NotFound();
            }
            List<Company> companiesResult = companies.GetRange(startIndex, endIndex - 2);
            return Ok(companiesResult);
        }

        [HttpPut("{Id}")]
        public ActionResult<Company> UpdateName(string Id, CreateCompanyRequest request)
        {
            Company? company = companies.FirstOrDefault(c => c.Id.Equals(Id));
            if (company == null)
            {
                return NotFound();
            }
            company.Name = request.Name;
            return Ok(company);
        }

        [HttpPost("{companyID}/employees")]
        public ActionResult<Employee> AddEmployee(string companyID, CreateEmployeeRequest request)
        {
            Company company = companies.FirstOrDefault(c => c.Id == companyID);
            if (company == null)
            {
                return NotFound(companyID);
            }
            if (company.Employees.Exists(e => e.Name == request.Name))
            {
                return BadRequest();
            }
            Employee employeeAdded = new Employee(request.Name);
            company.Employees.Add(employeeAdded);
            return StatusCode(StatusCodes.Status201Created, employeeAdded);
        }


        [HttpDelete("{companyID}/employees/{employeeID}")]
        public ActionResult<Employee> DeleteEmployee(string companyID, string employeeID)
        {
            Company company = companies.FirstOrDefault(c => c.Id == companyID);
            if (company == null)
            {
                return NotFound(companyID);
            }
            Employee employeeDeleted = company.Employees.FirstOrDefault(e => e.Id == employeeID);
            if (employeeDeleted == null)
            {
                return NotFound();
            }
            company.Employees.Remove(employeeDeleted);
            return Ok(employeeDeleted);
        }
    }
}

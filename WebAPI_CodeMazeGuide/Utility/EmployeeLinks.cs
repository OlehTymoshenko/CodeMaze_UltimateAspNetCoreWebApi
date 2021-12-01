using Contracts;
using Entities.DTOs;
using Entities.Models;
using Entities.Models.LinkModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Net.Http;

namespace WebAPI_CodeMazeGuide.Utility
{
    public class EmployeeLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeDTO> _dataShaper;

        public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDTO> dataShaper) =>
            (_linkGenerator, _dataShaper) = (linkGenerator, dataShaper);

        public LinkResponse TryGenerateLinks(
            IEnumerable<EmployeeDTO> employees,
            string fields,
            Guid companyId,
            HttpContext httpContext)
        {
            var shapedEmployees = ShapeData(employees, fields);

            if (ShouldGenerateLinks(httpContext).GetValueOrDefault())
                return CreateLinkedEmployees(employees, fields, companyId, httpContext, shapedEmployees);

            return CreateShapedEmployees(shapedEmployees);
        }


        private List<Entity> ShapeData(IEnumerable<EmployeeDTO> employees, string requiredFields) =>
            _dataShaper.ShapeData(employees, requiredFields)
            .Select(e => e.Entity)
            .ToList();

        private bool? ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = httpContext.Items["AcceptHeaderMediaType"] as MediaTypeHeaderValue;

            return mediaType?.SubTypeWithoutSuffix.EndsWith("hateoas", 
                StringComparison.InvariantCultureIgnoreCase);
        }

        private LinkResponse CreateShapedEmployees(List<Entity> shapedEmployees) =>
            new LinkResponse 
            { 
                ShapedEntities = shapedEmployees,
                HasLinks = false
            };

        private LinkResponse CreateLinkedEmployees(
            IEnumerable<EmployeeDTO> employees, 
            string fields, 
            Guid companyId, 
            HttpContext httpContext, 
            List<Entity> shapedEmployees)
        {
            var employeeDtoList = employees.ToList();

            for (int i = 0; i < employeeDtoList.Count; i++)
            {
                var employeeLinks = CreateLinksForEmployee(httpContext, companyId,
                    employeeDtoList[i].Id, fields);

                shapedEmployees[i].Add("Links", employeeLinks);
            }

            var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);
            var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);

            return new LinkResponse
            {
                HasLinks = true,
                LinkedEntities = linkedEmployees
            };
        }

        private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext, LinkCollectionWrapper<Entity> employeeCollection)
        {
            employeeCollection.Links.Add(new Link 
            {
                Href = _linkGenerator.GetUriByAction(httpContext, "GetEmployeesForCompanyAsync"),
                Rel = "self",
                Method = HttpMethod.Get
            });

            return employeeCollection;
        }

        private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId, Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link
                {
                    Href = _linkGenerator.GetUriByAction(httpContext, "GetEmployeeByIdAsync",
                        values: new { companyId, id, fields }),
                    Rel = "self",
                    Method = HttpMethod.Get
                },
                new Link
                {
                    Href = _linkGenerator.GetUriByAction(httpContext, "DeleteEmployeeAsync"),
                    Rel = "delete_employee",
                    Method = HttpMethod.Delete
                },
                new Link
                {
                    Href = _linkGenerator.GetUriByAction(httpContext, "UpdateAsync",
                        values: new { companyId, id }),
                    Rel = "update_employee",
                    Method = HttpMethod.Put
                },
                new Link
                {
                    Href = _linkGenerator.GetUriByAction(httpContext, "PatchAsync",
                        values: new { companyId, id }),
                    Rel = "partially_update_employee",
                    Method = HttpMethod.Patch
                }
            };

            return links;
        }
    }
}

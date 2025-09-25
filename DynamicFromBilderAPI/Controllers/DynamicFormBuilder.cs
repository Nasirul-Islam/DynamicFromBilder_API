using DynamicFromBilderAPI.Data;
using DynamicFromBilderAPI.Models; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DynamicFromBilderAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class DynamicFormBuilder : ControllerBase
    {
        private readonly MasterSpRepository _repository;

        public DynamicFormBuilder(MasterSpRepository repository)
        {
            _repository = repository;
        }

		// Get all form fields
		[HttpGet("AllFormFields")]
        public IActionResult GetFormFields()
        { 
            DataSet ds = _repository.ExecuteMasterSP(
							procName: "SP_DYNAMIC_FROM_BILDER_APPLICATION",
							procId: "GET_FORMFIELDS_DATA"
                       );

			List<CodeBook> codeBookItems = new List<CodeBook>();
			if (ds.Tables.Count > 0)
			{
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					codeBookItems.Add(new CodeBook
					{
						CodeId = Convert.ToInt32(row["CodeId"]),
						InputType = row["InputType"].ToString(),
						DefaultLabel = row["DefaultLabel"].ToString(),
						Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null
					});
				}
			}
			return Ok(codeBookItems);
        }

        // save dynamic form
        [HttpPost("SaveForm")]
        public IActionResult SubmitForm([FromBody] FormSubmitDto request)
        {
            try
            {
                string fieldsJson = Newtonsoft.Json.JsonConvert.SerializeObject(request.Fields);

                _repository.ExecuteMasterSP(
                    procName: "SP_DYNAMIC_FROM_BILDER_APPLICATION",
                    procId: "SAVE_FORM_DATA",
                    djson1: fieldsJson,
                    desc01: request.Title
                );

                return Ok(new { success = true, message = "Form saved successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // get all submitted dynamic forms 
        [HttpGet("AllForms")]
        public IActionResult GetAllForms()
        {
            try
            {
                DataSet ds = _repository.ExecuteMasterSP(
                    procName: "SP_DYNAMIC_FROM_BILDER_APPLICATION",
                    procId: "GET_ALL_FORMS"
                );

                var forms = new List<object>();
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        forms.Add(new
                        {
                            FormId = Convert.ToInt32(row["FormId"]),
                            Title = row["Title"].ToString(),
                            FieldsJson = row["FieldsJson"].ToString(),
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                        });
                    }
                }

                return Ok(forms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // get a single form data
        [HttpGet("Form/{id}")]
        public IActionResult GetFormById(int id)
        {
            try
            {
                // Call stored procedure with procId and formId
                DataSet ds = _repository.ExecuteMasterSP(
                    procName: "SP_DYNAMIC_FROM_BILDER_APPLICATION",
                    procId: "GET_FORM_BY_ID",
                    desc01: id.ToString()   // passing formId as Desc01
                );

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return NotFound(new { success = false, message = "Form not found" });

                DataRow row = ds.Tables[0].Rows[0];

                var form = new
                {
                    FormId = Convert.ToInt32(row["FormId"]),
                    Title = row["Title"].ToString(),
                    FieldsJson = row["FieldsJson"].ToString(),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                };

                return Ok(form);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        //  server site pagination
        [HttpPost("GetFormsData")]
        public IActionResult GetFormsData()
        {
            try
            {
                // DataTables parameters
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Convert.ToInt32(HttpContext.Request.Form["start"].FirstOrDefault() ?? "0");
                var length = Convert.ToInt32(HttpContext.Request.Form["length"].FirstOrDefault() ?? "10");
                var searchValue = HttpContext.Request.Form["search[value]"].FirstOrDefault();

                // Call SP to get all forms 
                DataSet ds = _repository.ExecuteMasterSP(
                    procName: "SP_DYNAMIC_FROM_BILDER_APPLICATION",
                    procId: "GET_ALL_FORMS"
                );

                var forms = new List<dynamic>();
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        forms.Add(new
                        {
                            FormId = Convert.ToInt32(row["FormId"]),
                            Title = row["Title"].ToString(),
                            FieldsJson = row["FieldsJson"].ToString(),
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                        });
                    }
                }

                // Searching  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    forms = forms
                        .Where(f => f.Title.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var recordsTotal = forms.Count;

                // Pagination
                var data = forms.Skip(start).Take(length).ToList();

                // DataTables response format
                return Ok(new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.ToString() });
            }

        }


    }
}

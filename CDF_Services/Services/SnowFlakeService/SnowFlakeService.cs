using CDF_Core.Entities.SnowFlake;
using CDF_Services.IServices.ISnowFlakeService;
using Snowflake.Data.Client;
using System.Data.Common;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDF_Services.Services.SnowFlakeService
{
    public class SnowFlakeService : ISnowFlakeService
    {

        public async Task<IActionResult> ListEmployeeDetail()
        {
            var employees = new List<Employee>();
            try
            {
                using (IDbConnection conn = new SnowflakeDbConnection())
                {
                    //conn.ConnectionString = "account=pz05486;user=hirendevani;Password=Surat@8505;warehouse=my_warehouse;database=EMPLOYEEINFODB;schema=EMPLOYEE;role=ACCOUNTADMIN";
                    //conn.ConnectionString = "account=pz05486.central-india.azure.snowflakecomputing.com;user=HIRENDEVANI;password=Surat@8505;db=EMPLOYEEINFODB;schema=EMPLOYEE;warehouse=my_warehouse;role=ACCOUNTADMIN";
                    //conn.ConnectionString = "account=EH47344;user=HIRENDEVANI;password=Surat@8505";
                    conn.ConnectionString = "Host=pz05486.central-india.azure.snowflakecomputing.com;User=hirendevani;Password=Surat@8505;Account=pz05486;Db=EMPLOYEEINFODB;schema=EMPLOYEE;warehouse=my_warehouse;Role=ACCOUNTADMIN";
                   
                    conn.Open();
                    Console.WriteLine("Connection successful!");

                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        //  var query = "SELECT * FROM EMPLOYEEINFODB.EMPLOYEE.EMPLOYEE";
                        var query = "SELECT * FROM EMPLOYEEINFODB.EMPLOYEE.EMPLOYEE";

                        cmd.CommandText = "USE WAREHOUSE my_warehouse";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = query;  // sql opertion fetching 
                                                  //data from an existing table
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                    Name = reader.GetString(reader.GetOrdinal("NAME")),
                                    Email = reader.GetString(reader.GetOrdinal("EMAIL")),
                                    Dept = reader.GetString(reader.GetOrdinal("DEPT")),
                                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CREATED_AT"))
                                };

                                employees.Add(employee);
                            }
                        }
                        conn.Close();

                    }
                }
            }
            catch (Exception exc)
            {
                
                Console.WriteLine("Error Message: {0}", exc.Message);
                return new JsonResult(new { StatusCode = 400, Error = exc.Message });

            }

            return new JsonResult(new { StatusCode = 200, Data = employees });
        }


        
    }
    }


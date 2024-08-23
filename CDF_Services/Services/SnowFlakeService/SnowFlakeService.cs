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
                    conn.ConnectionString = "Account=TLUNZQI.EH47344;User=hirendevani;Password=Surat@8505;Warehouse=my_warehouse;Database=EMPLOYEEINFODB;Schema=EMPLOYEE;Role=ACCOUNTADMIN";
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

           /* try
            {
                using (var conn = new SnowflakeDbConnection { ConnectionString = "Account=TLUNZQI.EH47344;User=hirendevani;Password=Surat@8505;Warehouse=my_warehouse;Database=EMPLOYEEINFODB;Schema=EMPLOYEE;Role=ACCOUNTADMIN;" })
                {
                    // Open the connection asynchronously
                    await conn.OpenAsync();
                    var query = "SELECT * FROM EMPLOYEEINFODB.EMPLOYEE.EMPLOYEE";

                    using (var cmd = conn.CreateCommand())
                    {
                      
                        cmd.CommandText = query;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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
                    }

              *//*      // Define the SQL query
                    var query = "SELECT * FROM EMPLOYEEINFODB.EMPLOYEE.EMPLOYEE";

                    // Initialize SnowflakeDbCommand with the query and connection
                    using (var command = new SnowflakeDbCommand(query, conn))
                    {
                        // Execute the query and retrieve the data
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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
                    }*//*
                }
            }
            catch (SnowflakeDbException ex)
            {
                // Log detailed SnowflakeDbException
                // Example: _logger.LogError(ex, "SnowflakeDbException occurred while querying the database.");
                throw new ApplicationException("A Snowflake database error occurred.", ex);
            }
            catch (Exception ex)
            {
                // Log detailed general exception
                // Example: _logger.LogError(ex, "An error occurred while querying the database.");
                throw new ApplicationException("An error occurred while retrieving data.", ex);
            }*/



            return new JsonResult(new { StatusCode = 200, Data = employees });
        }


        public async Task<IActionResult> ListEmployeeDetail1()
        {
            string version = "";
            var employees = new List<Employee>();
        try
        {
            using (var conn = new SnowflakeDbConnection { ConnectionString = "Account=TLUNZQI.EH47344;User=hirendevani;Password=Surat@8505;Warehouse=my_warehouse;Database=EMPLOYEEINFODB;Schema=EMPLOYEE;Role=ACCOUNTADMIN;"           })
            {
                await conn.OpenAsync();
                Console.WriteLine("Connection successful!");

                   
                using (var cmd = conn.CreateCommand())
                {
                    var query = "SELECT CURRENT_VERSION()"; // Test query
                    cmd.CommandText = query;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine($"Version: {reader.GetString(0)}");
                                version = reader.GetString(0);
                        }
                    }
                }
                }
            return new JsonResult(new { StatusCode = 200, Version = version });
        }
        catch (SnowflakeDbException sfEx)
        {
            Console.WriteLine($"Snowflake Error: {sfEx.Message}");
            return new JsonResult(new { StatusCode = 400, Error = sfEx.Message,Type="SnowFlake" });
        }
        catch (Exception exc)
        {
            Console.WriteLine($"General Error: {exc.Message}");
            return new JsonResult(new { StatusCode = 400, Error = exc.Message,Type= "General" });
        }
    }
    }
}

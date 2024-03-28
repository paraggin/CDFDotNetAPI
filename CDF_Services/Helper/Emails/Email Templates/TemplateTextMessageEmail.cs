using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Helper.Emails.Email_Templates
{

    public static class TemplateTextMessage
    {
        public static string GetTemplate(string Subject, string Message)
        {
            string str = $@"  


<!DOCTYPE html>
<html lang=""en"">

<head>
	<meta charset=""UTF-8"">
	<!-- Change Site Title -->
	<title>CDF</title>
	<!-- Change Page Title-->
	<link rel=""icon"" type=""image/ico"" href="""" />
	<!-- Google Fonts-->
	<link href=""https://fonts.googleapis.com/css2?family=Tajawal:wght@400;500;700;800;900&display=swap""
		rel=""stylesheet"">
</head>

<body style=""padding: 0; margin: 0; background-color: #eee;"">
	<!-- Start Verify Email Page -->
	<table width=""600px"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0"">

		<tbody>
			<!-- END Page Logo -->
			<!-- START Page Header -->
			<tr>
				<td align=""center"">
					<table align=""center"" class=""col-600"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">
						<tbody>
							<tr>
								<td align=""center"" bgcolor=""#fff"">
									<table class=""col-600"" width=""600px"" align=""center"" border=""0"" cellspacing=""0""
										cellpadding=""0"">
										<tbody>
											<tr>
												<td>
													<table class=""col3_one"" width=""100%"" border=""0"" align=""right""
														cellpadding=""0"" cellspacing=""0"">
														<tbody>
															<tr>
																<td style=""text-align: center; padding-top: 20px; padding-bottom: 15px;""
																	bgcolor=""#eeeeee"">
																	<img src=""cid:logo"">
																</td>
															</tr>
															<tr align=""left"" valign=""top"">
																<td>
																	<div class=""page-title""
																		style=""text-align: start; margin: 40px 0; border-radius: 8px; padding: 40px 30px; text-align: center;"">
																		<h3
																			style=""font-family: 'Tajawal', sans-serif; color: #404040; font-size: 25px; margin-bottom: 5px;"">
																			{Subject}
																		</h3>
																		<p
																			style=""font-family: 'Tajawal', sans-serif; color: #404040; font-size: 17px; margin-bottom: 20px; font-weight: 500;"">
																			{Message}
																		</p>
																	</div>
																</td>
															</tr>
														</tbody>
													</table>
												</td>
											</tr>
										</tbody>
									</table>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
			<tr>
				<td align=""center"">
					<table align=""center"" class=""col-600"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">
						<tbody>
							<tr>
								<td bgcolor=""#fff"">
									<div style=""display: flex; align-items: center; justify-content: center;"">
										<a href="""" style=""display: inline-block; margin: 5px;"">
											<img src=""cid:linkedin"">
										</a>
										<a href="""" style=""display: inline-block; margin: 5px;"">
											<img src=""cid:twitter"">
										</a>
										<a href="""" style=""display: inline-block; margin: 5px;"">
											<img src=""cid:facebook"">
										</a>
										<a href="""" style=""display: inline-block; margin: 5px;"">
											<img src=""cid:pinterest"">
										</a>
									</div>
								</td>
							</tr>
							<tr>
								<td bgcolor=""#101828"">
									<div>
										<p
											style=""font-family: 'Tajawal', sans-serif; color: #fff; font-size: 17px; font-weight: 500; text-align: center;"">
											© جميع الحقوق محفوظة © 2023 إيزي فينيو
										</p>
									</div>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
			<!-- END Page Content -->
		</tbody>
	</table>
	<!-- End Verify Email Page -->
</body>

</html>
            ";
            return str;


        }
    }
}

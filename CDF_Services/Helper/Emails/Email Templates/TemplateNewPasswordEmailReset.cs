using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Helper.Emails.Email_Templates
{
    public static class TemplateNewPasswordEmailReset
    {
        public static string GetTemplate(string Password)
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
								<td align=""center"" bgcolor=""#eeeeee"">
									<table class=""col-600"" width=""600px"" align=""center"" border=""0"" cellspacing=""0""
										cellpadding=""0"">
										<tbody>
											<tr>
												<td>
													<table class=""col3_one"" width=""100%"" border=""0"" align=""right""
														cellpadding=""0"" cellspacing=""0"">
														<tbody>
															<tr align=""left"" valign=""top"">
																<td>
																	<div class=""page-title""
																		style=""text-align: start; margin: 40px 0; background-color: #fff;border-radius: 8px; padding: 70px 30px; text-align: center;"">
																		<img src=""cid:logo"">

																		<div style=""margin-top: 40px;"">
																			<img src=""cid:PassImage"">
																		</div>

																		<h3
																			style=""font-family: 'Tajawal', sans-serif; color: #404040; font-size: 25px; margin-bottom: 5px;"">
																			كلمة المرور الجديدة
																		</h3>
																		<p
																			style=""font-family: 'Tajawal', sans-serif; color: #404040; font-size: 17px; margin-bottom: 20px; font-weight: 500;"">
																			لقد تلقينا طلبًا لإعادة تعيين كلمة المرور
																			لحسابك
																		</p>
																		<p
																			style=""font-family: 'Tajawal', sans-serif; color: #404040; font-size: 27px; margin-bottom: 40px; font-weight: 500;"">
																			<strong>{Password}</strong>
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
								<td bgcolor=""#eee"">
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
									<div>
										<p
											style=""font-family: 'Tajawal', sans-serif; color: #404040; font-size: 17px; margin-bottom: 40px; font-weight: 500; text-align: center;"">
											© جميع الحقوق محفوظة © 2023 إيزي ستف
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

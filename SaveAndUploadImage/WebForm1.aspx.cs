using System;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SaveAndUploadImage
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            HttpPostedFile postedFile = FileUpload1.PostedFile;
            string filename = Path.GetFileName(postedFile.FileName);
            string fileExtension = Path.GetExtension(filename);
            int filesize = postedFile.ContentLength;


            if( fileExtension.ToLower() == ".jpg" || fileExtension.ToLower() == ".bmp" || fileExtension.ToLower() == ".gif" || fileExtension.ToLower() == ".png")
            {
                Stream stream = postedFile.InputStream;
                BinaryReader binaryReader = new BinaryReader(stream);
                byte[] bytes = binaryReader.ReadBytes((int)stream.Length);

                string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("spUploadImage",con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramName = new SqlParameter()
                    {
                        ParameterName = "@Name",
                        Value = filename

                    };
                    cmd.Parameters.Add(paramName);


                    SqlParameter paramSize = new SqlParameter()
                    {
                        ParameterName = "@Size",
                        Value = filesize

                    };
                    cmd.Parameters.Add(paramSize);

                    SqlParameter paramImageData = new SqlParameter()
                    {
                        ParameterName = "@ImageData",
                        Value = bytes

                    };
                    cmd.Parameters.Add(paramImageData);

                    SqlParameter paramNew = new SqlParameter()
                    {
                        ParameterName = "@NewId",
                        Value = -1,
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramNew);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();


            
                    Label1.Text = "Uploaded Sucessfully";
                    Label1.ForeColor = System.Drawing.Color.Green;
                    HyperLink1.Visible = true;
                    HyperLink1.NavigateUrl = "~/Webform2.aspx?Id=" + cmd.Parameters["@NewId"].Value.ToString();
                }
            }
            else
            {
            
                Label1.Text = "Only .jpg, .bmp, .png and .gif  formats are applicable";
                Label1.ForeColor = System.Drawing.Color.Red;
            
            }
        }
    }
}
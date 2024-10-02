using System;




namespace WebAPITemplate.Swagger
{
    public class SwaggerExampleJsonRequestAttribute : Attribute
    {

        private string filename;
        private string requestHeader;
        public SwaggerExampleJsonRequestAttribute(string filename, string requestHeader="")
        {
            this.filename = filename;
            this.requestHeader = requestHeader;
        }

        public virtual string FileName
        {
            get { return filename; }
        }
        public virtual string RequestHeader
        {
            get { return requestHeader; }
        }
    }
}

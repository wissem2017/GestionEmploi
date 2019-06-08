namespace GestionEmploi.API.Helpers
{
    //--> Paramétrage de page user
    public class MessageParams
    {
       
        private const int MaxPageSize = 50; //--> Max users par page
        public int PageNumber { get; set; }=1;
        private int pageSize = 10; //--> Nbre des users qui seront afficher par page
        public int  PageSize
        {
            get { return pageSize;}
            set { pageSize =(value > MaxPageSize)?MaxPageSize:value;}
        }

         public int UserId { get; set; }
         public string MessageType { get; set; }="Unread";
    }
}
namespace GestionEmploi.API.Helpers
{
    //--> Paramétrage de page user
    public class UserParams
    {
        private const int MaxPageSize = 50; //--> Max users par page
        public int PageNumber { get; set; }=1;
        private int pageSize = 10; //--> Nbre des users qui seront afficher par page
        public int  PageSize
        {
            get { return pageSize;}
            set { pageSize =(value > MaxPageSize)?MaxPageSize:value;}
        }

        //--> User connecter ne sera pas afficher dans la liste 
        public int UserId { get; set; }
        public string Gender { get; set; }

        public int MinAge { get; set; }=18;
        public int MaxAge { get; set; }=99;

        public string OrderBy {get; set;}

        public bool Likees { get; set; }=false;
        public bool Likers { get; set; }=false;
        

    }
}
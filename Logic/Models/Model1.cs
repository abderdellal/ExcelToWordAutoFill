namespace Logic.Models
{
    using Migrations;
    using System.Data.Entity;

    public class Model1 : DbContext
    {
        public Model1()
            : base("name=SqlCE")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Model1, Configuration>());
        }

        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<KeyColumnPair> KeyColumnPairs { get; set; }
    }

}
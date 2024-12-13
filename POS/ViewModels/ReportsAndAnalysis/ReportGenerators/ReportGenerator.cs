using DataAccess;

namespace POS.ViewModels.ReportsAndAnalysis.ReportGenerators
{
    public abstract class ReportGenerator(AppDbContext dbContext)
    {
        protected readonly AppDbContext _dbContext = dbContext;
    }
}

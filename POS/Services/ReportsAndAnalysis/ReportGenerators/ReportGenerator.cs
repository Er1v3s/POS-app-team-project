using DataAccess;

namespace POS.Services.ReportsAndAnalysis.ReportGenerators
{
    public abstract class ReportGenerator(AppDbContext dbContext)
    {
        protected readonly AppDbContext _dbContext = dbContext;
    }
}

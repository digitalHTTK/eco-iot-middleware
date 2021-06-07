using Microsoft.EntityFrameworkCore;
using Plan_io_T.Models;

namespace Plan_io_T.Data {
    public class MvcDataContext : DbContext {
        public MvcDataContext(DbContextOptions<MvcDataContext> options)
            : base(options) { }
        public DbSet<ArduinoData> ArduinoData { get; set; }
    }
}

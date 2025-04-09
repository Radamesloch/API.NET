using System.ComponentModel.DataAnnotations;

namespace Vendsys.Models
{
    public class DexLaneMeter
    {
        [Key]
        public int Id { get; set; }
        public string ProductId { get; set; }
        public decimal Price { get; set; }
        public int NumberVends { get; set; }
        public decimal ValuePaidSales { get; set; }
        public int IdDexMeter { get; set; }

    }

    public class DexLaneMaterList
    {
        public DexLaneMaterList()
        {
            DexLaneList = new List<DexLaneMeter>();
        }
        public List<DexLaneMeter> DexLaneList {get;set;} 
    }

}

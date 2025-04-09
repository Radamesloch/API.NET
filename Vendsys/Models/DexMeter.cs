using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vendsys.Models
{
    public class DexMeter
    {        
        public DexMeter(string guid) {
            this.Guid = guid;
        }
        [Key]        
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Machine {  get; set; }
        public DateTime DexDatetime { get; set; }
        public string MachineSerialNumber { get; set; }
        public string ValuePaidVends {  get; set; }
    }
}

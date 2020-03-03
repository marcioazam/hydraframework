using HydraFramework;
using HydraFramework.Attributes;
using HydraFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [Table]
    public class Cliente : HydraModel
    {
        [PK]
        public int ID { get; set; }
        
        [Column]
        public string Nome { get; set; }
        
        [Column]
        public string Endereco { get; set; }
        
        [Column]
        public int Idade { get; set; }
        
        [Column]
        public decimal Dinheiro { get; set; }
        
        [Column]
        public bool Humano { get; set; }

        public List<Pedido> pedidos
        {
            get => _pedidos.Value;
            set => _pedidos = new Lazy<List<Pedido>>(() => value);
        }
        private Lazy<List<Pedido>> _pedidos { get; set; }
    }

    [Table]
    public class Pedido
    {
        [PK]
        public int ID { get; set; }

        [Column]
        public string Nome { get; set; }

        [Column]
        public decimal Valor { get; set; }

        [Column]
        public int ClienteID { get; set; }
    }

    [Table]
    public class AGR_ETAPA
    {
        [PK]
        public int ETA_ID { get; set; }

        [Column]
        public string ETA_NOME { get; set; }

        [Column]
        public int ETA_ORDEM { get; set; }

        [Column]
        public int PRC_ID { get; set; }

        public Lazy<List<AGR_ATIVIDADE>> atividades { get; set; }

        public AGR_ETAPA()
        {
            Hydra hydra = new Hydra("data source=localhost;initial catalog = agronomica; persist security info = True;Integrated Security = SSPI");

            atividades = new Lazy<List<AGR_ATIVIDADE>>(() => hydra.Load<AGR_ATIVIDADE>(condition: "WHERE ETA_ID = 6"));
        }

    }

    [Table]
    public class AGR_ATIVIDADE
    {
        [PK]
        public int ATV_ID { get; set; }
        
        [Column]
        public int ETA_ID { get; set; }
        
        [Column]
        public int USU_ID_ABERTURA { get; set; }
        
        [Column]
        public int? USU_ID_FECHAMENTO { get; set; }
        
        [Column]
        public DateTime ATV_DATA_ABERTURA { get; set; }
        
        [Column]
        public DateTime? ATV_DATA_FECHAMENTO { get; set; }
        
        [Column]
        public int ATV_STATUS { get; set; }
        
        [Column]
        public int PRT_ID { get; set; }
        
        [Column]
        public DateTime? DataCadastro { get; set; }
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            Hydra hydra = new Hydra("data source=localhost;initial catalog = agronomica; persist security info = True;Integrated Security = SSPI");

            var eta6 = hydra.LoadSingle<AGR_ETAPA>(condition: "WHERE ETA_ID = 6");



            var atividadesEta6 = eta6.atividades.Value;

            //var item = hydra.Load<Pedido>(top: 2, condition: "WHERE ClienteID = " + 1);

            //cliente[0].pedidos = item;

            //var pedidos = cliente[0].pedidos;


        }
    }
}

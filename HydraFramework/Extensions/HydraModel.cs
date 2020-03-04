using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using HydraFramework.Attributes;
using HydraFramework.Modules;
using static HydraFramework.Modules.Carrega;
using static HydraFramework.Modules.Manipula;

namespace HydraFramework.Extensions
{
    public abstract class HydraModel : INotifyPropertyChanged
    {
        [Browsable(false)]
        public DataContext Contexto { get; set; }

        [Browsable(false)]
        public event PropertyChangedEventHandler PropertyChanged;

        [Browsable(false)]
        private List<PropertyInfo> autoJoins
        {
            get
            {
                return GetAutoJoin();
            }
        }

        [Browsable(false)]
        private PropertyInfo GetChaveEstrangeira<T>()
        {
            Type tipo = typeof(T);
            T entidade = (T)Activator.CreateInstance(tipo);

            PropertyInfo properties = entidade.GetType().GetProperties().Where(x => Valida.ForeignKey(x) != null).FirstOrDefault();

            return properties;
        }

        [Browsable(false)]
        private Dictionary<PropriedadePK, object> propriedadesPK
        {
            get
            {
                return Carrega.InfoPrimaryKey(this, this.GetType(), GetPK());
            }
        }

        [Browsable(false)]
        private PropertyInfo GetPK()
        {
            PropertyInfo properties = GetPropriedades().Where(x => Valida.PrimaryKey(x) != null).ToArray().FirstOrDefault();

            return properties;
        }

        private List<PropertyInfo> GetAutoJoin()
        {
            List<PropertyInfo> autoJoins = new List<PropertyInfo>();

            PropertyInfo[] properties = GetPropriedades().Where(x => Valida.AutoJoin(x) != null).ToArray();

            foreach (var prop in properties)
            {
                autoJoins.Add(prop);
            }

            return autoJoins;
        }

        [Browsable(false)]
        private PropertyInfo[] GetPropriedades()
        {
            PropertyInfo[] properties = this.GetType().GetProperties();

            return properties;
        }

        protected List<T> AutoJoin<T>()
        {
            Hydra hydra = new Hydra(new DataContext(""));

            string chaveEstrangeiraNome = GetChaveEstrangeira<T>().Name;
            object valorChavePrimaria = propriedadesPK[PropriedadePK.Valor];

            HydraParameters hydraParameters = new HydraParameters();
            hydraParameters.Add("@ValorChavePrimaria", valorChavePrimaria);

            string where = $"WHERE {chaveEstrangeiraNome} = @ValorChavePrimaria";

            return hydra.Load<T>(condition: where, parameters: hydraParameters);
        }

    }
}

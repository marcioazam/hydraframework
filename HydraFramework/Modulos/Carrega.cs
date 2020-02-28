using HydraFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace HydraFramework.Modulos
{
    internal class Carrega
    {
        public enum PropriedadePK
        {
            Nome = 1,
            Valor = 2,
            Identidade = 3
        }

        public static Dictionary<PropriedadePK, object> PropriedadesPrimaryKey(MemberInfo membro, object entidade)
        {
            Dictionary<PropriedadePK, object> keyValues = new Dictionary<PropriedadePK, object>();
            var PK = (PKAttribute)Attribute.GetCustomAttribute(membro, typeof(PKAttribute));
            string NomePK;

            keyValues.Add(PropriedadePK.Identidade, PK.Identity);

            if (PK.Name != "")
            {
                NomePK = PK.Name;
            }
            else
            {
                NomePK = membro.Name;
            }

            keyValues.Add(PropriedadePK.Nome, NomePK);
            keyValues.Add(PropriedadePK.Valor, entidade.GetType().GetProperty(NomePK).GetValue(entidade, null));

            return keyValues;
        }

        public static void ColunasDataTable(DataTable dataTable, Type tipo)
        {
            PropertyInfo[] propertyInfo = tipo.GetProperties().Where(x => Valida.Coluna(x) != null || Valida.PrimaryKey(x) != null).ToArray();

            foreach (PropertyInfo propriedade in propertyInfo)
            {
                string nomeColuna = Valida.NomeColuna(propriedade);

                Type tipoColuna = propriedade.PropertyType;

                tipoColuna = Valida.TipoNull(tipoColuna);

                DataColumn dataColumn = new DataColumn(nomeColuna, tipoColuna);

                dataTable.Columns.Add(dataColumn);
            }
        }

        public static List<string> Parametros(object entidade, Type tipo, HydraParameters parametros, PropertyInfo[] colunas)
        {
            List<string> listaNomeColunas = new List<string>();

            foreach (var coluna in colunas)
            {
                var nomeColuna = Valida.NomeColuna(coluna);
                var valor = entidade.GetType().GetProperty(coluna.Name).GetValue(entidade, null);

                parametros.Add(nomeColuna, valor);
                listaNomeColunas.Add(nomeColuna);
            }

            return listaNomeColunas;
        }

        public static Dictionary<PropriedadePK, object> InfoPrimaryKey(object entidade, Type tipo, HydraParameters parametros, PropertyInfo PK)
        {
            Dictionary<PropriedadePK, object> keyValues = null;
            
            if (PK != null)
            {
                keyValues = Carrega.PropriedadesPrimaryKey(PK, entidade);
                var nomeColuna = keyValues[PropriedadePK.Nome];
                var valor = keyValues[PropriedadePK.Valor];

                parametros.AddPK(PK.Name, valor);
            }

            return keyValues;
        }
    }
}

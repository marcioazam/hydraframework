using HydraFramework.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace HydraFramework.Modules
{
    static internal class Valida
    {
        public static object PrimaryKey(PropertyInfo propriedade)
        {
            var atributos = propriedade.GetCustomAttributes(true).ToList();

            var attributoPK = atributos.Where(x => x.GetType().Name == "PKAttribute").FirstOrDefault();

            return attributoPK;
        }

        public static object AutoJoin(PropertyInfo propriedade)
        {
            var atributos = propriedade.GetCustomAttributes(true).ToList();

            var attributoColuna = atributos.Where(x => x.GetType().Name == "AutoJoinAttribute").FirstOrDefault();

            return attributoColuna;
        }

        public static object ForeignKey(PropertyInfo propriedade)
        {
            var atributos = propriedade.GetCustomAttributes(true).ToList();

            var attributoColuna = atributos.Where(x => x.GetType().Name == "FKAttribute").FirstOrDefault();

            return attributoColuna;
        }

        public static object Coluna(PropertyInfo propriedade)
        {
            var atributos = propriedade.GetCustomAttributes(true).ToList();

            var attributoColuna = atributos.Where(x => x.GetType().Name == "ColumnAttribute").FirstOrDefault();

            return attributoColuna;
        }

        public static string NomeTabela(Type tipo)
        {
            var dados = (TableAttribute)Attribute.GetCustomAttribute(tipo, typeof(TableAttribute));

            try
            {
                if (dados.Name != "")
                {
                    return dados.Name;
                }
                else
                {
                    return tipo.Name;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string NomeColuna(MemberInfo membro)
        {
            var dadosColuna = (ColumnAttribute)Attribute.GetCustomAttribute(membro, typeof(ColumnAttribute));
            var dadosFK = (FKAttribute)Attribute.GetCustomAttribute(membro, typeof(FKAttribute));

            if (dadosColuna != null)
            {
                if (dadosColuna.Name != "")
                {
                    return dadosColuna.Name;
                }
                else
                {
                    return membro.Name;
                }
            }
            else if(dadosFK != null)
            {
                if (dadosFK.Name != "")
                {
                    return dadosFK.Name;
                }
                else
                {
                    return membro.Name;
                }
            }
            else
            {
                var dadosPK = (PKAttribute)Attribute.GetCustomAttribute(membro, typeof(PKAttribute));

                if (dadosPK.Name != "")
                {
                    return dadosPK.Name;
                }
                else
                {
                    return membro.Name;
                }
            }
        }

        public static Type TipoNull(Type tipoColuna)
        {
            if (tipoColuna.IsGenericType && tipoColuna.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                tipoColuna = tipoColuna.GetGenericArguments()[0];
            }

            return tipoColuna;
        }
    }
}

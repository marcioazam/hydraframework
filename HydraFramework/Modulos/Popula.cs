using HydraFramework.Modulos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace HydraFramework.Modulos
{
    internal class Popula
    {
        public static T Entidade<T>(SqlDataReader dadosTabela)
        {
            var tipo = typeof(T);

            PropertyInfo[] propertyInfo = tipo.GetProperties().Where(x => Valida.Coluna(x) != null || Valida.PrimaryKey(x) != null).ToArray();

            T entidade;

            while (dadosTabela.Read())
            {
                if (tipo.Name == "String")
                {
                    entidade = (T)Assembly.GetExecutingAssembly().CreateInstance(tipo.FullName);
                }
                else
                {
                    entidade = (T)Activator.CreateInstance(tipo);
                }

                if (propertyInfo.Count() > 0)
                {
                    foreach (var property in propertyInfo)
                    {
                        var nomeColuna = Valida.NomeColuna(property);

                        var valor = dadosTabela[nomeColuna] != DBNull.Value ? dadosTabela[nomeColuna] : null;

                        property.SetValue(entidade, valor, null);
                    }
                }
                else
                {
                    var valor = dadosTabela[0] != DBNull.Value ? dadosTabela[0] : null;

                    object genericoTipificado = valor;

                    entidade = (T)Convert.ChangeType(genericoTipificado, typeof(T));
                }

                return entidade;
            }

            return (T)Activator.CreateInstance<T>();
        }

        public static List<T> Lista<T>(SqlDataReader dadosTabela) 
        {
            var lista = new List<T>();
            var tipo = typeof(T);

            PropertyInfo[] propertyInfo = tipo.GetProperties().Where(x => Valida.Coluna(x) != null || Valida.PrimaryKey(x) != null).ToArray();

            T entidade;

            while (dadosTabela.Read())
            {
                if (tipo.Name == "String")
                {
                    entidade = (T)Assembly.GetExecutingAssembly().CreateInstance(tipo.FullName);
                }
                else
                {
                    entidade = (T)Activator.CreateInstance(tipo);
                }

                if (propertyInfo.Count() > 0)
                {
                    foreach (var property in propertyInfo)
                    {
                        var nomeColuna = Valida.NomeColuna(property);

                        var valor = dadosTabela[nomeColuna] != DBNull.Value ? dadosTabela[nomeColuna] : null;

                        property.SetValue(entidade, valor, null);
                    }
                }
                else
                {
                    var valor = dadosTabela[0] != DBNull.Value ? dadosTabela[0] : null;

                    object genericoTipificado = valor;

                    entidade = (T)Convert.ChangeType(genericoTipificado, typeof(T));
                }

                lista.Add(entidade);
            }

            return lista;
        }

        public static object Objeto(SqlDataReader dadosTabela, object entidade, Type tipo)
        {
            PropertyInfo[] propertyInfo = tipo.GetProperties().Where(x => Valida.Coluna(x) != null || Valida.PrimaryKey(x) != null).ToArray();

            if (propertyInfo.Count() > 0)
            {
                while (dadosTabela.Read())
                {
                    foreach (var property in propertyInfo)
                    {
                        var nomeColuna = Valida.NomeColuna(property);

                        var valor = dadosTabela[nomeColuna] != DBNull.Value ? dadosTabela[nomeColuna] : null;

                        property.SetValue(entidade, valor, null);
                    }
                }
            }

            return entidade;
        }

        public static List<HydraTuple<T1, T2>> ListaDuasTuple<T1, T2>(SqlDataReader dadosTabela) 
        {
            var lista = new List<HydraTuple<T1, T2>>();
            var tipoTupla1 = Valida.TipoNull(typeof(T1));
            var tipoTupla2 = Valida.TipoNull(typeof(T2));

            T1 Tupla1;
            T2 Tupla2;

            while (dadosTabela.Read())
            {
                Tupla1 = NovaInstancia<T1>(tipoTupla1, dadosTabela[0]);
                Tupla2 = NovaInstancia<T2>(tipoTupla2, dadosTabela[1]);

                lista.Add(HydraTuple.New(Tupla1, Tupla2));
            }

            return lista;
        }

        public static List<HydraTuple<T1, T2, T3>> ListaTresTuple<T1, T2, T3>(SqlDataReader dadosTabela)
        {
            var lista = new List<HydraTuple<T1, T2, T3>>();
            var tipoTupla1 = Valida.TipoNull(typeof(T1));
            var tipoTupla2 = Valida.TipoNull(typeof(T2));
            var tipoTupla3 = Valida.TipoNull(typeof(T3));

            T1 Tupla1;
            T2 Tupla2;
            T3 Tupla3;

            while (dadosTabela.Read())
            {
                Tupla1 = NovaInstancia<T1>(tipoTupla1, dadosTabela[0]);
                Tupla2 = NovaInstancia<T2>(tipoTupla2, dadosTabela[1]);
                Tupla3 = NovaInstancia<T3>(tipoTupla3, dadosTabela[2]);

                lista.Add(HydraTuple.New(Tupla1, Tupla2, Tupla3));
            }

            return lista;
        }

        private static T NovaInstancia<T>(Type tipo, object dadosTabela)
        {
            T entidade;

            if (tipo.Name == "String")
            {
                entidade = (T)Assembly.GetExecutingAssembly().CreateInstance(tipo.FullName); 
            }
            else
            {
                entidade = (T)Activator.CreateInstance(tipo);
            }

            entidade = SetaValorInstancia(dadosTabela, entidade, tipo);

            return entidade;
        }

        private static T SetaValorInstancia<T>(object dadosTabela, T entidade, Type tipo)
        {
            var valor = dadosTabela != DBNull.Value ? dadosTabela : null;

            object genericoTipificado = valor;

            if(genericoTipificado != null)
            {
                entidade = (T)Convert.ChangeType(genericoTipificado, tipo);
            }
            else
            {
                entidade = default(T);
            }

            return entidade;
        }

        public static DataTable DataTable(SqlDataReader dadosTabela)
        {
            DataTable esquemaTabela = dadosTabela.GetSchemaTable();
            DataTable dataTable = new DataTable();
            DataColumn dataColumn;
            DataRow dataRow;

            for (int i = 0; i < esquemaTabela.Rows.Count; i++)
            {
                dataColumn = new DataColumn();
                if (!dataTable.Columns.Contains(esquemaTabela.Rows[i]["ColumnName"].ToString()))
                {
                    dataColumn.ColumnName = esquemaTabela.Rows[i]["ColumnName"].ToString();
                    dataColumn.Unique = Convert.ToBoolean(esquemaTabela.Rows[i]["IsUnique"]);
                    dataColumn.AllowDBNull = Convert.ToBoolean(esquemaTabela.Rows[i]["AllowDBNull"]);
                    dataColumn.ReadOnly = Convert.ToBoolean(esquemaTabela.Rows[i]["IsReadOnly"]);
                    dataTable.Columns.Add(dataColumn);
                }
            }

            while (dadosTabela.Read())
            {
                dataRow = dataTable.NewRow();
                for (int i = 0; i < esquemaTabela.Rows.Count; i++)
                {
                    dataRow[i] = dadosTabela.GetValue(i);
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public static void Parametros(HydraParameters parametros, SqlCommand comando, bool PKIdentidade)
        {
            if (parametros != null)
            {
                List<SqlParameter> sqlParameters;

                if (PKIdentidade == true)
                {
                    sqlParameters = parametros.ReturnParameters();
                }
                else
                {
                    sqlParameters = parametros.ReturnWithPK();
                }

                if (sqlParameters.Count > 0)
                {
                    foreach (var parametro in sqlParameters)
                    {
                        comando.Parameters.AddWithValue(parametro.ParameterName, parametro.SqlValue ?? (object)DBNull.Value);
                    }
                }
            }
        }
    }
}

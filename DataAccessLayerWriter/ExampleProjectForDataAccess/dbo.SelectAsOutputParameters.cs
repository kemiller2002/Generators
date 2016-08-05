

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Net.Configuration;
using System.Linq;





                    namespace dbo
                    {
                        public class SelectAsOutputParametersResult
                        {
                            public int RecordsAffected {get;set;}
                    
                            public Int32 @Int {get;set;}
public Guid @Guid {get;set;}
public String @VarChar {get;set;}
public String @nVarchar {get;set;}
public Int16 @smallInt {get;set;}
public Byte @timyInt {get;set;}
public DateTime @DateTime {get;set;}
public DateTime @DateTime2 {get;set;}
public DateTime @SmallDateTime {get;set;}
public Double @float {get;set;}
public Decimal @numeric {get;set;}
public Single @real {get;set;}
                        }

                    }
                

namespace dbo
{

    public class SelectAsOutputParameters
    {

        readonly SqlConnection _connection;

        public SelectAsOutputParameters(SqlConnection connection)
        {
            _connection = connection;
        }

        public dbo.SelectAsOutputParametersResult Execute(Int32? @Int, Guid? @Guid, String @VarChar, String @nVarchar, Int16? @smallInt, Byte? @timyInt, DateTime? @DateTime, DateTime? @DateTime2, DateTime? @SmallDateTime, Double? @float, Decimal? @numeric, Single? @real)
        {
            var command = new SqlCommand
            {
                Connection = _connection,
                CommandText = "[dbo].[SelectAsOutputParameters]",
                CommandType = CommandType.StoredProcedure
            };


            
            if (@Int.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @Int,
                    ParameterName = "@Int"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.Int 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@Int"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 4
                    , SqlDbType =  SqlDbType.Int 
                };

                command.Parameters.Add(parameter);
                

            }

            if (@Guid.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @Guid,
                    ParameterName = "@Guid"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.UniqueIdentifier 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@Guid"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 32
                    , SqlDbType =  SqlDbType.UniqueIdentifier 
                };

                command.Parameters.Add(parameter);
                

            }


            
            if(@VarChar != null)
            {
                var @VarCharParameter = new SqlParameter
                {
                     Value = @VarChar,
                    ParameterName = "@VarChar"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.VarChar 
                };
                
                command.Parameters.Add(@VarCharParameter);
            }
            else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@VarChar"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 100
                    , SqlDbType =  SqlDbType.VarChar 
                };

                command.Parameters.Add(parameter);
                

            }
            


            
            if(@nVarchar != null)
            {
                var @nVarcharParameter = new SqlParameter
                {
                     Value = @nVarchar,
                    ParameterName = "@nVarchar"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.NVarChar 
                };
                
                command.Parameters.Add(@nVarcharParameter);
            }
            else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@nVarchar"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 100
                    , SqlDbType =  SqlDbType.NVarChar 
                };

                command.Parameters.Add(parameter);
                

            }
            

            if (@smallInt.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @smallInt,
                    ParameterName = "@smallInt"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.SmallInt 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@smallInt"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 2
                    , SqlDbType =  SqlDbType.SmallInt 
                };

                command.Parameters.Add(parameter);
                

            }

            if (@timyInt.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @timyInt,
                    ParameterName = "@timyInt"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.TinyInt 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@timyInt"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 1
                    , SqlDbType =  SqlDbType.TinyInt 
                };

                command.Parameters.Add(parameter);
                

            }

            if (@DateTime.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @DateTime,
                    ParameterName = "@DateTime"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.DateTime 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@DateTime"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 3
                    , SqlDbType =  SqlDbType.DateTime 
                };

                command.Parameters.Add(parameter);
                

            }

            if (@DateTime2.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @DateTime2,
                    ParameterName = "@DateTime2"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.DateTime2 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@DateTime2"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 8
                    , SqlDbType =  SqlDbType.DateTime2 
                };

                command.Parameters.Add(parameter);
                

            }

            if (@SmallDateTime.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @SmallDateTime,
                    ParameterName = "@SmallDateTime"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.SmallDateTime 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@SmallDateTime"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 4
                    , SqlDbType =  SqlDbType.SmallDateTime 
                };

                command.Parameters.Add(parameter);
                

            }

            if (@float.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @float,
                    ParameterName = "@float"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.Float 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@float"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 8
                    , SqlDbType =  SqlDbType.Float 
                };

                command.Parameters.Add(parameter);
                

            }

            if (@numeric.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @numeric,
                    ParameterName = "@numeric"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.Decimal 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@numeric"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 17
                    , SqlDbType =  SqlDbType.Decimal 
                };

                command.Parameters.Add(parameter);
                

            }

            if (@real.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = @real,
                    ParameterName = "@real"
                    , Direction = ParameterDirection.InputOutput 
                    , SqlDbType =  SqlDbType.Real 
                };

                command.Parameters.Add(parameter);
                
            }else {
                    var parameter = new SqlParameter
                {
                    ParameterName = "@real"
                    , Direction = ParameterDirection.InputOutput 
                    , Size = 4
                    , SqlDbType =  SqlDbType.Real 
                };

                command.Parameters.Add(parameter);
                

            }


            var result = command.ExecuteNonQuery();
                    
                return new dbo.SelectAsOutputParametersResult
                {
                    @Int = (Int32)command.Parameters["@Int"].Value,
@Guid = (Guid)command.Parameters["@Guid"].Value,
@VarChar = (String)command.Parameters["@VarChar"].Value,
@nVarchar = (String)command.Parameters["@nVarchar"].Value,
@smallInt = (Int16)command.Parameters["@smallInt"].Value,
@timyInt = (Byte)command.Parameters["@timyInt"].Value,
@DateTime = (DateTime)command.Parameters["@DateTime"].Value,
@DateTime2 = (DateTime)command.Parameters["@DateTime2"].Value,
@SmallDateTime = (DateTime)command.Parameters["@SmallDateTime"].Value,
@float = (Double)command.Parameters["@float"].Value,
@numeric = (Decimal)command.Parameters["@numeric"].Value,
@real = (Single)command.Parameters["@real"].Value,
RecordsAffected = result
                };
                
        }

    }

}


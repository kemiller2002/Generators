

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using StructuredSight.Data;




                    namespace dbo
                    {
                        public class SelectAsOutputParametersResult
                        {
                            public int RecordsAffected {get;set;}
                    
                            public Int32 Int {get;set;}
public Guid Guid {get;set;}
public String VarChar {get;set;}
public String NVarchar {get;set;}
public Int16 SmallInt {get;set;}
public Byte TimyInt {get;set;}
public DateTime DateTime {get;set;}
public DateTime DateTime2 {get;set;}
public DateTime SmallDateTime {get;set;}
public Double Float {get;set;}
public Decimal Numeric {get;set;}
public Single Real {get;set;}
                        }

                    }
                

namespace dbo
{

    public class SelectAsOutputParametersExecutionObject 
    {
        Int32? Int {get;set;}
Guid? Guid {get;set;}
String VarChar {get;set;}
String NVarchar {get;set;}
Int16? SmallInt {get;set;}
Byte? TimyInt {get;set;}
DateTime? DateTime {get;set;}
DateTime? DateTime2 {get;set;}
DateTime? SmallDateTime {get;set;}
Double? Float {get;set;}
Decimal? Numeric {get;set;}
Single? Real {get;set;}    
    }

    public class SelectAsOutputParameters : StructuredSight.Data.BaseAccess<SelectAsOutputParametersResult>
    {

        public SelectAsOutputParameters(SqlConnection connection) : base(connection)
        {
        }

        public dbo.SelectAsOutputParametersResult Execute(Int32? int, Guid? guid, String varChar, String nVarchar, Int16? smallInt, Byte? timyInt, DateTime? dateTime, DateTime? dateTime2, DateTime? smallDateTime, Double? float, Decimal? numeric, Single? real)
        {
            var command = Connection.CreateCommand();
            command.CommandText = "[dbo].[SelectAsOutputParameters]";
            command.CommandType = CommandType.StoredProcedure;


            
            if (int.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = int,
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

            if (guid.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = guid,
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


            
            if(varChar != null)
            {
                var @VarCharParameter = new SqlParameter
                {
                     Value = varChar,
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
            


            
            if(nVarchar != null)
            {
                var @nVarcharParameter = new SqlParameter
                {
                     Value = nVarchar,
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
            

            if (smallInt.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = smallInt,
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

            if (timyInt.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = timyInt,
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

            if (dateTime.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = dateTime,
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
                    , Size = 8
                    , SqlDbType =  SqlDbType.DateTime 
                };

                command.Parameters.Add(parameter);
                

            }

            if (dateTime2.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = dateTime2,
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

            if (smallDateTime.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = smallDateTime,
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

            if (float.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = float,
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

            if (numeric.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = numeric,
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

            if (real.HasValue)
            {
                var parameter = new SqlParameter
                {
                    Value = real,
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
                    Int = (Int32)command.Parameters["@Int"].Value,
Guid = (Guid)command.Parameters["@Guid"].Value,
VarChar = (String)command.Parameters["@VarChar"].Value,
NVarchar = (String)command.Parameters["@nVarchar"].Value,
SmallInt = (Int16)command.Parameters["@smallInt"].Value,
TimyInt = (Byte)command.Parameters["@timyInt"].Value,
DateTime = (DateTime)command.Parameters["@DateTime"].Value,
DateTime2 = (DateTime)command.Parameters["@DateTime2"].Value,
SmallDateTime = (DateTime)command.Parameters["@SmallDateTime"].Value,
Float = (Double)command.Parameters["@float"].Value,
Numeric = (Decimal)command.Parameters["@numeric"].Value,
Real = (Single)command.Parameters["@real"].Value,
RecordsAffected = result
                };
                
        }

        public dbo.SelectAsOutputParametersResult ExecuteWithExecutionObject(SelectAsOutputParametersExecutionObject input)
        {
            
            return Execute(int : input.Int, guid : input.Guid, varChar : input.VarChar, nVarchar : input.NVarchar, smallInt : input.SmallInt, timyInt : input.TimyInt, dateTime : input.DateTime, dateTime2 : input.DateTime2, smallDateTime : input.SmallDateTime, float : input.Float, numeric : input.Numeric, real : input.Real);
        
        }

    }

}


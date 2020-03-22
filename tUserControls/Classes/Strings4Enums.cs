using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tracker.UserControls.Classes
{
    public static class Strings4Enums
    {
        #region Enums

        public enum qFieldType : byte
        {
            iDateTime = 4,
            iNumber = 1,
            iText = 2,
            iBoolean = 3
        }

        public enum qLogOper
        {
            //Query LogicalOperators 
            iOpenParenthesis = 0,   //(
            iCloseParenthesis = 1,  //)
            iAnd = 2,        //and
            iAndNot = 3,    //and not
            iAndBetParentheses = 4,   //) and (
            iAndNotBetParentheses = 5,  //) and Not(
            iOr = 6,           //or
            iOrNot = 7,       //or not
            iOrBetParentheses = 8,  //) or (
            iOrNotBetParentheses = 9  //) or Not(
        }

        public enum qClauses
        {
            BeginsWith = 1,
            Between = 2,
            Contains = 3,
            EndsWith = 4,
            Equal = 5,
            GreaterThan = 6,
            GreaterThanOrEqualTo = 7,
            iIn = 8,
            IsEmpty = 9,
            IsNull = 10,
            LessThan = 11,
            LessThanOrEqualTo = 12,
            NotBetween = 13,
            NotContains = 14,
            NotEqual = 15,
            NotIn = 16,
            NotIsEmpty = 17,
            NotIsNull = 18,
            RegularExpression = 19,
            NotBeginsWith = 20,
            NotEndsWith = 21,
            iTrue = 29,
            iFalse = 30,
            Custom = 31
        }

        #endregion

        public static qFieldType dotNetType_to_BasicType(TypeCode tyCode)
        {
            switch (tyCode)
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return qFieldType.iNumber;
                case TypeCode.Boolean: return qFieldType.iBoolean;
                case TypeCode.DateTime: return qFieldType.iDateTime;
                case TypeCode.Char:
                case TypeCode.String:
                case TypeCode.Object:
                    return qFieldType.iText;
                default:
                    MessageBox.Show("Type not Mapped : '" + tyCode.GetType().FullName + "'");
                    return qFieldType.iText;
            }
        }
        //internal static qFieldType dotNetType_to_BasicType(Type type)
        //{
        //    throw new NotImplementedException();
        //}

        public static qFieldType dotNetType_to_BasicType(Type testObject)
        {
            TypeCode tyCode = Type.GetTypeCode(testObject);
            return dotNetType_to_BasicType(tyCode);
        }


        /// <summary>
        /// return a string value
        /// </summary>
        /// <param name="Clause"></param>
        /// <returns>string</returns>
        public static string sClause(qClauses Clause)
        {
            switch (Clause)
            {
                case qClauses.BeginsWith: return "Begins With";
                case qClauses.NotBeginsWith: return "not Begins With";
                case qClauses.Between: return "Between";
                case qClauses.Contains: return "Contains";
                case qClauses.EndsWith: return "Ends With";
                case qClauses.NotEndsWith: return "not Ends With";
                case qClauses.Equal: return "Equal";
                case qClauses.GreaterThan: return "GreaterThan";
                case qClauses.GreaterThanOrEqualTo: return "Greater Than Or Equal To";
                case qClauses.iFalse: return "False";
                case qClauses.iIn: return "In";
                case qClauses.IsEmpty: return "is Empty";
                case qClauses.IsNull: return "is Null";
                case qClauses.iTrue: return "True";
                case qClauses.LessThan: return "Less Than";
                case qClauses.LessThanOrEqualTo: return "Less Than Or Equal To";
                case qClauses.NotBetween: return "Not Between";
                case qClauses.NotContains: return "Not Contains";
                case qClauses.NotEqual: return "Not Equal";
                case qClauses.NotIn: return "Not In";
                case qClauses.NotIsEmpty: return "Not Is Empty";
                case qClauses.NotIsNull: return "Not Is Null";
                case qClauses.Custom: return "Custom";
                case qClauses.RegularExpression: return "Matches Regular Expression";
            }
            return "";
        }   //a FieldType Text will have a <> set vs. Number or Boolean

        public static string sLogOper(qLogOper AndOr)
        {
            switch (AndOr)
            {
                case qLogOper.iAnd: return " AND ";
                case qLogOper.iAndBetParentheses: return " ) AND ( ";
                case qLogOper.iAndNot: return " AND NOT ";
                case qLogOper.iAndNotBetParentheses: return " ) AND NOT ( ";
                case qLogOper.iCloseParenthesis: return " ) ";
                case qLogOper.iOpenParenthesis: return " ( ";
                case qLogOper.iOr: return " OR ";
                case qLogOper.iOrBetParentheses: return " ) OR ( ";
                case qLogOper.iOrNot: return " OR NOT ";
                case qLogOper.iOrNotBetParentheses: return " ) OR NOT ( ";
            }
            return null;
        }

        public static List<qClauses> Ft4Clauses(qFieldType ColType, bool CaseSensitive = false)
        {
            List<qClauses> aClauses = null;
            switch (ColType)
            {
                case qFieldType.iText:
                    aClauses = new List<qClauses>(new qClauses[]{
            qClauses.Contains,
            qClauses.BeginsWith,
            qClauses.EndsWith,
            qClauses.Equal,
            qClauses.NotContains,
            qClauses.NotEqual,
            qClauses.IsNull,
            qClauses.NotIsNull,
            qClauses.iIn,
            qClauses.NotIn,
            qClauses.Custom,
            qClauses.RegularExpression
                   });
                    break;
                case qFieldType.iNumber:
                    aClauses = new List<qClauses>(new qClauses[]{
            qClauses.Equal,
            qClauses.GreaterThan,
            qClauses.LessThan,
            qClauses.GreaterThanOrEqualTo,
            qClauses.LessThanOrEqualTo,
            qClauses.NotBetween,
            qClauses.Between,
            qClauses.NotEqual,
            qClauses.IsNull,
            qClauses.NotIsNull,
            qClauses.iIn,
            qClauses.NotIn,
            qClauses.Custom,
            qClauses.RegularExpression
        });
                    break;
                case qFieldType.iBoolean:
                    aClauses = new List<qClauses>(new qClauses[]{
            qClauses.Equal,
            qClauses.iTrue,
            qClauses.iFalse,
            qClauses.IsNull,
            qClauses.NotIsNull,
            qClauses.Custom
        });
                    break;
                case qFieldType.iDateTime:
                    aClauses = new List<qClauses>(new qClauses[]{
            qClauses.Equal,
            qClauses.GreaterThan,
            qClauses.LessThan,
            qClauses.GreaterThanOrEqualTo,
            qClauses.LessThanOrEqualTo,
            qClauses.Between,
            qClauses.NotBetween,
            qClauses.Contains,
            qClauses.BeginsWith,
            qClauses.EndsWith,
            qClauses.NotContains,
            qClauses.NotEqual,
            qClauses.IsNull,
            qClauses.NotIsNull,
            qClauses.iIn,
            qClauses.NotIn,
            qClauses.Custom
        });

                    break;
            }
            return aClauses;
        }


        public static string sqlClauses(qClauses Clause, bool CaseSensitive,  qFieldType at,
            MyDb.Common.DataBaseType _cnnType = MyDb.Common.DataBaseType.ORACLE)
        {

            if (CaseSensitive)
            {
                switch (Clause)
                {
                    case qClauses.BeginsWith:
                        return "\"??_Attr_Name\" like 'XX_VALUE_FROM%' ";
                    case qClauses.Between:
                        if (_cnnType == MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" between to_date('XX_VALUE_FROM','MM/DD/YYYY') and to_date('YY_VALUE_TO','MM/DD/YYYY') "
                            : "\"??_Attr_Name\" between XX_VALUE_FROM and YY_VALUE_TO ";
                        }
                        else if (_cnnType == MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" between CONVERT(DATETIME,'XX_VALUE_FROM') and CONVERT(DATETIME,'YY_VALUE_TO') "
                           : "\"??_Attr_Name\" between XX_VALUE_FROM and YY_VALUE_TO ";
                        }
                        else { return "\"??_Attr_Name\" between 'XX_VALUE_FROM' and 'YY_VALUE_TO' "; }
                    case qClauses.Contains:
                        return "\"??_Attr_Name\" like '%XX_VALUE_FROM%' ";
                    case qClauses.EndsWith:
                        return "\"??_Attr_Name\" like '%XX_VALUE_FROM' ";
                    case qClauses.Equal:
                        if (_cnnType == MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" = to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "\"??_Attr_Name\" = 'XX_VALUE_FROM' ";
                        }
                        if (_cnnType == MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" = CONVERT(DATETIME,'XX_VALUE_FROM') " : "\"??_Attr_Name\" = 'XX_VALUE_FROM' ";
                        }
                        return "\"??_Attr_Name\" = 'XX_VALUE_FROM' ";
                    case qClauses.GreaterThan:
                        if (_cnnType == MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" > to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "\"??_Attr_Name\" > 'XX_VALUE_FROM' ";
                        }
                        if (_cnnType == MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" > CONVERT(DATETIME,'XX_VALUE_FROM') " : "\"??_Attr_Name\" > 'XX_VALUE_FROM' ";
                        }
                        return "\"??_Attr_Name\" > 'XX_VALUE_FROM' ";
                    case qClauses.GreaterThanOrEqualTo:
                        if (_cnnType == MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" >= to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "\"??_Attr_Name\" >= 'XX_VALUE_FROM' ";
                        }
                        if (_cnnType == MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" >= CONVERT(DATETIME,'XX_VALUE_FROM') " : "\"??_Attr_Name\" >= 'XX_VALUE_FROM' ";
                        }
                        return "\"??_Attr_Name\" >= 'XX_VALUE_FROM' ";
                    case qClauses.iFalse:
                        //= 0 "
                        return "\"??_Attr_Name\" = (1=2)";
                    case qClauses.iIn:
                        return "\"??_Attr_Name\" IN (XX_VALUE_FROM) ";
                    case qClauses.IsEmpty:
                        return "\"??_Attr_Name\" = '' ";
                    case qClauses.IsNull:
                        return "\"??_Attr_Name\" IS NULL";
                    case qClauses.iTrue:
                        //<> 0"
                        return "\"??_Attr_Name\" = (1=1)";
                    case qClauses.LessThan:
                        if (_cnnType == MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" < to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "\"??_Attr_Name\" < 'XX_VALUE_FROM' ";
                        }
                        if (_cnnType == MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" < CONVERT(DATETIME,'XX_VALUE_FROM') " : "\"??_Attr_Name\" < 'XX_VALUE_FROM' ";
                        }
                        return "\"??_Attr_Name\" < 'XX_VALUE_FROM' ";
                    case qClauses.LessThanOrEqualTo:
                        if (_cnnType == MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" <= to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "\"??_Attr_Name\" <= 'XX_VALUE_FROM' ";
                        }
                        if (_cnnType == MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" <= CONVERT(DATETIME,'XX_VALUE_FROM') " : "\"??_Attr_Name\" <= 'XX_VALUE_FROM' ";
                        }
                        return "\"??_Attr_Name\" <= 'XX_VALUE_FROM' ";
                    case qClauses.NotBetween:
                        if (_cnnType == MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "NOT \"??_Attr_Name\" between to_date('XX_VALUE_FROM','MM/DD/YYYY') and to_date('YY_VALUE_TO','MM/DD/YYYY') "
                            : "NOT \"??_Attr_Name\" between XX_VALUE_FROM and YY_VALUE_TO ";
                        };
                        if (_cnnType == MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "NOT \"??_Attr_Name\" between CONVERT(DATETIME,'XX_VALUE_FROM') and CONVERT(DATETIME,'YY_VALUE_TO') "
                            : "NOT \"??_Attr_Name\" between XX_VALUE_FROM and YY_VALUE_TO ";
                        }
                        return "NOT \"??_Attr_Name\" between XX_VALUE_FROM and YY_VALUE_TO "; ;
                    case qClauses.NotContains:
                        return " NOT \"??_Attr_Name\" like '%XX_VALUE_FROM%' ";
                    case qClauses.NotEqual:
                        if (_cnnType == MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" <> to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "\"??_Attr_Name\" <> 'XX_VALUE_FROM' ";
                        }
                        if (_cnnType == MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "\"??_Attr_Name\" <> CONVERT(DATETIME,'XX_VALUE_FROM') " : "\"??_Attr_Name\" <> 'XX_VALUE_FROM' ";
                        }
                        return "\"??_Attr_Name\" <> 'XX_VALUE_FROM' ";
                    case qClauses.NotIn:
                        return "\"??_Attr_Name\" Not In (XX_VALUE_FROM)";
                    case qClauses.NotIsEmpty:
                        return "\"??_Attr_Name\" <> ''";
                    case qClauses.NotIsNull:
                        return " Not \"??_Attr_Name\" is null";
                    case qClauses.Custom:
                        return "XX_VALUE_FROM";
                    case qClauses.RegularExpression:
                        return "REGEXP_LIKE(??_Attr_Name, 'XX_VALUE_FROM','c') ";
                }
            }
            else
            {
                switch (Clause)
                {
                    case qClauses.BeginsWith:
                        return "??_Attr_Name like 'XX_VALUE_FROM%' ";
                    case qClauses.Between:
                        //return "??_Attr_Name between 'XX_VALUE_FROM' and 'YY_VALUE_TO' ";
                        //return at == qFieldType.iDateTime ? "??_Attr_Name between to_date('XX_VALUE_FROM','MM/DD/YYYY') and to_date('YY_VALUE_TO','MM/DD/YYYY') " : "??_Attr_Name between XX_VALUE_FROM and YY_VALUE_TO ";
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.ORACLE)
                        {
                            return "  ??_Attr_Name between to_date('XX_VALUE_FROM','MM/DD/YYYY') and to_date('YY_VALUE_TO','MM/DD/YYYY') ";
                        }
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return "  ??_Attr_Name between CONVERT(DATETIME,'XX_VALUE_FROM') and CONVERT(DATETIME,'YY_VALUE_TO') ";
                        }
                        return " ??_Attr_Name between XX_VALUE_FROM and YY_VALUE_TO ";
                    case qClauses.Contains:
                        return "??_Attr_Name like '%XX_VALUE_FROM%' ";
                    case qClauses.EndsWith:
                        return "??_Attr_Name like '%XX_VALUE_FROM' ";
                    case qClauses.Equal:
                        // return "??_Attr_Name = 'XX_VALUE_FROM' ";
                        if (_cnnType ==  MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "??_Attr_Name = to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "??_Attr_Name = 'XX_VALUE_FROM' ";
                        }
                        if (_cnnType ==  MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "??_Attr_Name = CONVERT(DATETIME,'XX_VALUE_FROM') " : "??_Attr_Name = 'XX_VALUE_FROM' ";
                        }
                        return "upper(??_Attr_Name) = upper('XX_VALUE_FROM') ";
                    case qClauses.GreaterThan:
                        //return "??_Attr_Name > XX_VALUE_FROM ";
                        if (_cnnType ==  MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "??_Attr_Name > to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "??_Attr_Name > XX_VALUE_FROM ";
                        }
                        if (_cnnType ==  MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "??_Attr_Name > CONVERT(DATETIME,'XX_VALUE_FROM') " : "??_Attr_Name > XX_VALUE_FROM ";
                        }
                        return "??_Attr_Name > XX_VALUE_FROM ";
                    case qClauses.GreaterThanOrEqualTo:
                        //return "??_Attr_Name >= XX_VALUE_FROM ";
                        if (_cnnType ==  MyDb.Common.DataBaseType.ORACLE)
                        {
                            return at == qFieldType.iDateTime ? "??_Attr_Name >= to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "??_Attr_Name >= 'XX_VALUE_FROM' ";
                        }
                        if (_cnnType ==  MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return at == qFieldType.iDateTime ? "??_Attr_Name >=  CONVERT(DATETIME,'XX_VALUE_FROM') " : "??_Attr_Name >= 'XX_VALUE_FROM' ";
                        }
                        return "??_Attr_Name >= 'XX_VALUE_FROM' ";
                    case qClauses.iFalse:
                        //0 "
                        return "??_Attr_Name = (1=2)";
                    case qClauses.iIn:
                        return "??_Attr_Name IN (XX_VALUE_FROM) ";
                    case qClauses.IsEmpty:
                        return "??_Attr_Name = '' ";
                    case qClauses.IsNull:
                        return "??_Attr_Name IS NULL";
                    case qClauses.iTrue:
                        //<> 0"
                        return "??_Attr_Name =(1=1)";
                    case qClauses.LessThan:
                        //return "??_Attr_Name < XX_VALUE_FROM ";
                        //return at == qFieldType.iDateTime ? "??_Attr_Name < to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "??_Attr_Name < 'XX_VALUE_FROM' ";
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.ORACLE)
                        {
                            return "??_Attr_Name < to_date('XX_VALUE_FROM','MM/DD/YYYY') ";
                        }
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return "??_Attr_Name < CONVERT(DATETIME,'XX_VALUE_FROM') ";
                        }
                        return "??_Attr_Name < XX_VALUE_FROM ";
                    case qClauses.LessThanOrEqualTo:
                        //return "??_Attr_Name <= XX_VALUE_FROM ";
                        //return at == qFieldType.iDateTime ? "??_Attr_Name <= to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "??_Attr_Name <= 'XX_VALUE_FROM' ";
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.ORACLE)
                        {
                            return "??_Attr_Name <= to_date('XX_VALUE_FROM','MM/DD/YYYY') ";
                        }
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return "??_Attr_Name <= CONVERT(DATETIME,'XX_VALUE_FROM') ";
                        }
                        return "??_Attr_Name <= XX_VALUE_FROM ";
                    case qClauses.NotBetween:
                        //return " NOT ??_Attr_Name between 'XX_VALUE_FROM' and 'YY_VALUE_TO' ";
                        //return at == qFieldType.iDateTime ? " NOT ??_Attr_Name between to_date('XX_VALUE_FROM','MM/DD/YYYY') and to_date('YY_VALUE_TO','MM/DD/YYYY') " : "??_Attr_Name between XX_VALUE_FROM and YY_VALUE_TO ";
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.ORACLE)
                        {
                            return " NOT ??_Attr_Name between to_date('XX_VALUE_FROM','MM/DD/YYYY') and to_date('YY_VALUE_TO','MM/DD/YYYY') ";
                        }
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return " NOT ??_Attr_Name between CONVERT(DATETIME,'XX_VALUE_FROM') and CONVERT(DATETIME,'YY_VALUE_TO') ";
                        }
                        return " NOT ??_Attr_Name between XX_VALUE_FROM and YY_VALUE_TO ";
                    case qClauses.NotContains:
                        return " NOT ??_Attr_Name like '%XX_VALUE_FROM%' ";
                    case qClauses.NotEqual:
                        //return "??_Attr_Name <> 'XX_VALUE_FROM' ";
                        //return at == qFieldType.iDateTime ? "??_Attr_Name <> to_date('XX_VALUE_FROM','MM/DD/YYYY') " : "upper(??_Attr_Name) <> upper('XX_VALUE_FROM') ";
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.ORACLE)
                        {
                            return "??_Attr_Name <> to_date('XX_VALUE_FROM','MM/DD/YYYY') ";
                        }
                        if (at == qFieldType.iDateTime && _cnnType ==  MyDb.Common.DataBaseType.SQLSERVER)
                        {
                            return "??_Attr_Name <> CONVERT(DATETIME,'XX_VALUE_FROM') ";
                        }
                        return "??_Attr_Name <> 'XX_VALUE_FROM' ";
                    case qClauses.NotIn:
                        return "??_Attr_Name Not In (XX_VALUE_FROM)";
                    case qClauses.NotIsEmpty:
                        return "??_Attr_Name <> ''";
                    case qClauses.NotIsNull:
                        return " Not ??_Attr_Name is null";
                    case qClauses.Custom:
                        return "XX_VALUE_FROM";
                    case qClauses.RegularExpression:
                        return "REGEXP_LIKE(??_Attr_Name, 'XX_VALUE_FROM','i') ";    //http://www.regular-expressions.info/oracle.html
                }
            }
            return "";
        }




    }

}

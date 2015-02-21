using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityRepository
{

    public abstract class BasePartQuery : IPartQuery
    {
        private string _fieldName;
        private object _fieldValue;

        public BasePartQuery(string fieldName, object fieldValue)
        {
            _fieldName = fieldName;
            _fieldValue = fieldValue;
        }

        public string FeildName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }
        public object FieldValue
        {
            get { return _fieldValue; }
            set { _fieldValue = value; }
        }
    }

    public class PartQuery : BasePartQuery
    {
        public PartQuery(string fieldName, object fieldValue)
            : base(fieldName, fieldValue)
        { }
    }

    public class AndQueryPart : BasePartQuery, IPartConditionQuery
    {
        public AndQueryPart(string fieldName, object fieldValue)
            : base(fieldName, fieldValue)
        { }
    }

    public class OrQueryPart : BasePartQuery, IPartConditionQuery
    {
        public OrQueryPart(string fieldName, object fieldValue)
            : base(fieldName, fieldValue)
        { }
    }
}

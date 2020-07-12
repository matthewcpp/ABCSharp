using System;
using System.Collections.Generic;
using System.Text;

namespace ABC
{
    public class TuneHeader
    {   enum InformationField
        {
            Area,
            Book,
            Composer,
            Discography,
            FileUrl,
            Group,
            History,
            Notes,
            Origin,
            Rhythm,
            Remark,
            Source,
            Title,
            ReferenceNumber,
            Transcription
        }

        private Dictionary<InformationField, string> fields = new Dictionary<InformationField, string>();

        private string getValue(InformationField field)
        {
            fields.TryGetValue(field, out string result);
            return result;
        }
        
        public string referenceNumber
        {
            get => getValue(InformationField.ReferenceNumber);
            set => fields[InformationField.ReferenceNumber] = value;
        }

        public string title
        {
            get => getValue(InformationField.Title);
            set => fields[InformationField.Title] = value;
        }

        public string composer
        {
            get => getValue(InformationField.Composer);
            set => fields[InformationField.Composer] = value;
        }
    }
}

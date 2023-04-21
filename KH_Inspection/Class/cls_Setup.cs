using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KH_Inspection
{
    class cls_Setup
    {
        /// <summary>
        /// 카메라 세팅
        /// </summary>
        public string[] m_config = new string[]
        {
            //내용                  ///제목             ///변수명
            "23AC927",              ///카메라 시리얼    
            "100.100.100.100",      ///PLC IP주소
            "1000",                 ///PLC Port주소
            "DIO000",               ///IO 이름
            
        };


        
    }
}

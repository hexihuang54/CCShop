using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using System.IO;

namespace CCShop.Common.Util
{
    public class ExcelUtil
    {
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static MemoryStream ExportToExcel(DataTable dt)
        {
            //创建Excel文件的对象  
            HSSFWorkbook book = new HSSFWorkbook();

            //添加一个sheet  
            ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题  
            IRow row1 = sheet1.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                row1.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
            }

            //给sheet1的每一行每一列赋值  
            for (int m = 0; m < dt.Rows.Count; m++)
            {
                IRow rowtemp = sheet1.CreateRow(m + 1);

                for (int n = 0; n < dt.Columns.Count; n++)
                {
                    rowtemp.CreateCell(n).SetCellValue(dt.Rows[m][n].ToString());
                }
            }

            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}

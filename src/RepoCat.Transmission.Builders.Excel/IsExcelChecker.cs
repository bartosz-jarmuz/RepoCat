// -----------------------------------------------------------------------
//  <copyright file="IsExcelChecker.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;

namespace RepoCat.Transmission.Builders.Excel
{
    public static class IsExcelChecker
    {
        public static bool IsExcelFile(string path)
        {
            if (
                path.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith(".xlsm", StringComparison.OrdinalIgnoreCase)
            )
            {
                return true;
            }

            return false;
        }
    }
}
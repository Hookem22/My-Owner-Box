using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Sheet
/// </summary>
public class Sheet : BaseClass<Sheet>
{
	public Sheet()
	{
	}

    public string Header { get; set; }

    public string Name { get; set; }

    public string Overview { get; set; }


}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.SqlClient;
using System.Drawing;
using System.Data;
using System.Net;
using System.Text;
using System.Xml.XPath;
using System.Xml;
using System.Globalization;

public partial class tableb : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (User.Identity.Name != "")
        {
            saved_ports();
            checkPublicPortfolios();
            dataStatus.Text = "";
            savePanel.Visible = true;
            publicPanel.Visible = false;
        }
        else
        {
            savePanel.Visible = false;
            publicPanel.Visible = true;
            displayPublicPortfolios();
        }
        if (!Page.IsPostBack)
            displayPortfolio();
    }

    protected void checkPublicPortfolios()
    {
        int i=0;
        int k = 999;
        SqlCommand mycom = new SqlCommand();
        SqlConnection con = new SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';");
        List<string> publicPortfolios = new List<string>();
        string[] portfolioName;
        int sqlResult = 0;

        con.Open();
        mycom.Connection = con;
        mycom.CommandText = "SELECT COUNT(*) FROM bobpar.publicPortfolios where userName='" + User.Identity.Name + "'";
        sqlResult = (int)mycom.ExecuteScalar();
        con.Close();
        if (sqlResult > 0)
        {
            con.Open();
            mycom.CommandText = "DELETE FROM bobpar.publicPortfolios where userName='" + User.Identity.Name + "'";
            sqlResult = mycom.ExecuteNonQuery();
            con.Close();
        }
        for (i = 1; i < Request.Form.Keys.Count; i++)
        {
            if (Request.Form.Keys[i].Contains("portfolioCheckBox"))
            {
                portfolioName = Request.Form.Keys[i].Split('_');
                if (Convert.ToString(Request.Form[i]) == "on")
                {
                    publicPortfolios.Add(portfolioName[1]);
                }
            }
        }
        if (publicPortfolios.Count > 0)
        {
            mycom.Parameters.Add("@uName", SqlDbType.VarChar, 50);
            mycom.Parameters.Add("@portName", SqlDbType.VarChar, 50);
            con.Open();
            for (i = 0; i < publicPortfolios.Count; i++)
            {
                mycom.Connection = con;
                mycom.Parameters["@uName"].Value = User.Identity.Name;
                mycom.Parameters["@portName"].Value = publicPortfolios[i];
                mycom.CommandText = "INSERT INTO bobpar.publicPortfolios(userName, portfolio) VALUES (@uName, @portName)";
                sqlResult = mycom.ExecuteNonQuery();
            }
            con.Close();
        }
    }

    protected void addRowClick(object sender, EventArgs e)
    {
        displayPortfolio();
    }

    protected void resetPortfolioClick(object sender, EventArgs e)
    {
        buildRow(1);
    }

    private void displayPortfolio()
    {
        //Creates the table after each page load. Generates the controls for the data to go into.
        // cells of rowtext
        // 0 - checkbox
        // 1 - ticker
        // 2 - # of shares
        // 3 - Share Price
        // 4 - Investment
        // 5 - investment cost
        // 6 - buy Date
        // 7 - sell date
        // 8 - Close Price
        // 9 - Close Cash Gain
        // 10 - Close percent Gain
        // 11 - Link
        // 12 - Notes
        
        string[] positionValues;
        positionValues = new string[8];

        string numShares = "";
        string investment = "";
        string investmentCost = "";
        string sharePrice = "";
        string buyDate = "";
        string sellDate = "";
        string note = "";
        string ticker = "";
        string closePrice = "";

        double sumInvest = 0;
        double sumValue = 0;
        //double closePrice = 0;

        List<int> deletedRows;

        int tablerow = 1;
        int deletedRowNum = 1;
        int i = 0;
        int rowNum = 1;

        yahooData stockInfo = new yahooData();

        i = 0;
        sumInvest = 0;
        sumValue = 0;
        portfolioTable.HorizontalAlign = HorizontalAlign.Center;
        deletedRows = new List<int>();

        //for (i = 1; i < Request.Form.Keys.Count - 1; i++)
        //{
        //    if (Convert.ToString(Request.Form["ctl00$MainContent$row_" + deletedRowNum + "Col_0"]) == "on")
        //        deletedRows.Add(deletedRowNum);
        //    deletedRowNum++;
        //}
        //tablerow = 1;
        //rowNum = 1;

        //for (i = 1; i < Request.Form.Keys.Count - 1; i++)
        //{
        //    rowNum = checkDeletedRow(rowNum,deletedRows); // keeps the right id 
        //    //if (Request.Form.Keys[i].Contains("Col_0"))
        //    //    i += 10;
        //    if (Request.Form.Keys[i].Contains("row_" + rowNum + "Col_1") &&
        //                (Request.Form.Keys[i].Contains("row_" + rowNum + "Col_13") == false))
        //    {
        //        ticker = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_1"]);
        //        if (ticker != "")
        //        {
        //            buildRow(tablerow, ticker, 99);
        //            tablerow++;
        //        }
        //        rowNum++;
        //    }
        //}
        //buildRow(tablerow);
        dataStatus.Text = "";
        dataStatus.Visible = false;
        try
        {
            for (i = 1; i < Request.Form.Keys.Count - 1; i++)
            {
                if (Convert.ToString(Request.Form["ctl00$MainContent$row_" + deletedRowNum + "Col_0"]) == "on")
                    deletedRows.Add(deletedRowNum);
                deletedRowNum++;
            }

            for (i = 1; i < Request.Form.Keys.Count - 1; i++)
            {
                rowNum = checkDeletedRow(rowNum, deletedRows); // keeps the right id 
                if (Request.Form.Keys[i].Contains("row_" + rowNum + "Col_1") &&
                    (Request.Form.Keys[i].Contains("row_" + rowNum + "Col_13") == false))
                {
                    ticker = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_1"]);
                    if (ticker != "")
                    {
                        numShares = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_2"]);
                        sharePrice = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_3"]);
                        investment = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_4"]);
                        investmentCost = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_5"]);
                        buyDate = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_6"]);
                        sellDate = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_7"]);
                        closePrice = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_8"]);
                        note = Convert.ToString(Request.Form["ctl00$MainContent$row_" + rowNum + "Col_12"]);

                        buyDate = checkDate(buyDate);
                        sellDate = checkDate(sellDate);
                        investment = checkNumber(investment);
                        // check for valid ticker
                        if (ticker == null)
                            buildRow(tablerow, "*", numShares, sharePrice, investment, investmentCost,
                                        buyDate, sellDate, closePrice, note);
                        //valid ticker now check for valid buy date
                        else if (buyDate == "" || buyDate.Contains("*"))
                        {
                            if (buyDate.Contains("*"))
                                buildRow(tablerow, ticker, numShares, sharePrice, investment, investmentCost,
                                            buyDate, sellDate, closePrice, note);
                            else
                            {
                                stockInfo = createYahooData(ticker, buyDate, sellDate);
                                if (stockInfo.close.Count != 0)
                                {
                                    buildRow(tablerow, ticker, numShares, sharePrice, investment, investmentCost,
                                                buyDate, sellDate, twoDecimal(stockInfo.close.Last()), note);
                                }
                                else
                                {
                                    if (ticker.Contains("*") == false)
                                        ticker += "*";
                                    buildRow(tablerow, ticker, numShares, sharePrice, investment, investmentCost,
                                            buyDate, sellDate, closePrice, note);
                                }
                            }
                        }
                        //valid ticker, buydate, check for valid investment
                        else if (investment == "" || investment.Contains("*"))
                        {
                            stockInfo = createYahooData(ticker, buyDate, sellDate);
                            if (investment.Contains("*") == false)
                                investment += "*";
                            if (stockInfo.close.Count != 0)
                            {
                                buildRow(tablerow, ticker, numShares, sharePrice, investment, investmentCost,
                                            buyDate, sellDate, twoDecimal(stockInfo.close.Last()), note);
                            }
                            else
                                buildRow(tablerow, ticker, numShares, sharePrice, investment, investmentCost,
                                        buyDate, sellDate, closePrice, note);
                        }
                        //valid ticker, buydate, investment, lets try and calcuate values
                        else
                        {
                            stockInfo = createYahooData(ticker, buyDate, sellDate);
                            if (stockInfo.close.Count == 0)
                            {
                                if (ticker.Contains("*") == false)
                                    ticker += "*";
                                buildRow(tablerow, ticker, numShares, sharePrice, investment, investmentCost,
                                        buyDate, sellDate, closePrice, note);
                            }
                            else
                            {
                                // to calculate a value we need a valid closePrice, SharePrice
                                closePrice = checkNumber(closePrice);
                                sharePrice = checkNumber(sharePrice);

                                if (closePrice.Contains("*") || sharePrice.Contains("*"))
                                    buildRow(tablerow, ticker, numShares, sharePrice, investment, investmentCost,
                                        buyDate, sellDate, closePrice, note);
                                else
                                {
                                    if (closePrice == "")
                                        closePrice = twoDecimal(stockInfo.close.Last());
                                    else closePrice = twoDecimal(Convert.ToDouble(closePrice));
                                    if (sharePrice == "")
                                    {
                                        try
                                        {   // try and find the buyDate in stockinfo
                                            sharePrice = Convert.ToString(stockInfo.close[stockInfo.find_date(buyDate)]);
                                        }
                                        catch
                                        {  // if buy date isn't in the stockinfo use the first one
                                            sharePrice = Convert.ToString(stockInfo.close.First());
                                        }
                                    }
                                    else sharePrice = twoDecimal(Convert.ToDouble(sharePrice));

                                    buildPosition(tablerow,
                                                  ticker,
                                                  numShares,
                                                  sharePrice,
                                                  investment,
                                                  investmentCost,
                                                  buyDate,
                                                  sellDate,
                                                  closePrice,
                                                  note,
                                                  ref sumInvest,
                                                  ref sumValue);
                                }
                            }
                        }
                        tablerow++;
                    }
                    rowNum++;
                }
            }
            buildRow(tablerow);
            totals(sumInvest, sumValue);
            portfolioTable.HorizontalAlign = HorizontalAlign.Center;
        }
        catch (WebException e)
        {
            dataStatus.Visible = true;
            dataStatus.Text = e.Message;
        }
    }

    string checkDate(string theDate)
    {
        try
        {
            if (theDate == "")
                return theDate;
            Convert.ToDateTime(theDate);
            return theDate;
        }
        catch
        {
            if (theDate.Contains("*"))
                return theDate;
            else return theDate + "*";
        }
    }

    string checkNumber(string theNumber)
    {
        try
        {
            if (theNumber == "")
                return theNumber;
            Convert.ToDouble(theNumber);
            return theNumber;
        }
        catch
        {
            if (theNumber.Contains("*"))
                return theNumber;
            else return theNumber + "*";
        }
    }

    int checkDeletedRow(int currentRow, List<int> deletedRows)  
    {
        int nextRow = currentRow;
        
        while (deletedRows.Contains(nextRow))
            nextRow++;
        return nextRow;
    }

    void buildPosition(int row, 
                       string ticker, 
                       string numShares, 
                       string sharePrice, 
                       string investment, 
                       string investmentCost, 
                       string buyDate, 
                       string sellDate,
                       string closePrice,
                       string note,
                       ref double sumInvest,
                       ref double sumValue)
    {
        int dNumShares = 0;
        double dSharePrice = 0;
        double dInvestment = 0;
        double dInvestmentCost = 0;
        double cashGain = 0;
        double percentGain = 0;
        double positionValue = 0;
        double dClosePrice = 0;

        //these values are good
        dClosePrice = Convert.ToDouble(closePrice);
        dInvestment = Convert.ToDouble(investment);
        dSharePrice = Convert.ToDouble(sharePrice);
        ticker = ticker.ToUpper();

        //checking these values selldate, investmentcost, numshares, 
        sellDate = checkDate(sellDate);
        investmentCost = checkNumber(investmentCost);
        numShares = checkNumber(numShares);
        
        // if any of them have bad inputs just dump it out
        if (sellDate.Contains("*") || investmentCost.Contains("*") || numShares.Contains("*"))
            buildRow(row, ticker, numShares, sharePrice, investment, investmentCost,
                buyDate, sellDate, closePrice, note);

        // so now we know everyone is cool as a jewl at least we hope they are :-)
        buyDate = dateFormat(buyDate);
        sellDate = dateFormat(sellDate);
        
        if (investmentCost != "")
            dInvestmentCost = Convert.ToDouble(investmentCost);
        if (numShares != "")
            dNumShares = Convert.ToInt16(numShares);
        else dNumShares = Convert.ToInt16((dInvestment-dInvestmentCost) / dSharePrice);
                   
        if (dNumShares > 0)
        {
            dInvestment = (dSharePrice * dNumShares) + dInvestmentCost;
            cashGain = ((dClosePrice - dSharePrice) * dNumShares) - dInvestmentCost;
            positionValue = dInvestment + cashGain;
            sumValue += positionValue;
            sumInvest += dInvestment;
            percentGain = ((positionValue - dInvestment) / dInvestment) * 100;
        }
        buildRow(row, 
                 ticker, 
                 dNumShares, 
                 dSharePrice, 
                 dInvestment, 
                 dInvestmentCost, 
                 buyDate, 
                 sellDate, 
                 dClosePrice, 
                 cashGain,
                 percentGain, 
                 positionValue,
                 note);
    }

    void createTextBoxCell(string value, 
                           int row, 
                           int col, 
                           TableRow theRow,                           
                           int boxWidth = 70)
    {
        TextBox newTextBox;
        TableCell acell;
        
        newTextBox = new TextBox();
        acell = new TableCell();
        newTextBox.ID = "row_" + row + "Col_" + col;
        newTextBox.Width = boxWidth;       
        acell.Width = boxWidth;       
        newTextBox.Font.Size = FontUnit.Medium;
        newTextBox.Text = value;
        acell.Controls.Add(newTextBox);
        Session[newTextBox.ID] = newTextBox.Text;
        theRow.Cells.Add(acell);
    }

    void createTextBoxCell(double value, int row, int col, TableRow theRow, int boxWidth = 70)
    {      
        if (value==0)
            createTextBoxCell("", row, col, theRow, boxWidth);
        else createTextBoxCell(twoDecimal(value), row, col, theRow, boxWidth);
    }

    void createTextBoxCell(int value, int row, int col, TableRow theRow, int boxWidth = 70)
    {
        if (value == 0)
            createTextBoxCell("", row, col, theRow, boxWidth);
        else createTextBoxCell(Convert.ToString(value), row, col, theRow, boxWidth);
    }

    void createTextBoxCell(DateTime value, int row, int col, TableRow theRow, int boxWidth = 70)
    {
        createTextBoxCell(Convert.ToString(value.ToString("MM/dd/yy")), row, col, theRow, boxWidth);
    }

    void createLabelCell(double value, int row, int col, TableRow theRow, bool colorEnable = true)
    {
        Label newLabel;
        TableCell acell;

        newLabel = new Label();
        acell = new TableCell();
        newLabel.ID = "row_" + row + "Col_" + col;
        newLabel.Width = 70;
        acell.Width = 70;
        newLabel.Font.Size = FontUnit.Medium;
        if (colorEnable == true)
        {
            if (value > 0)
                newLabel.ForeColor = Color.Green;
            else newLabel.ForeColor = Color.Red;
        }
        else newLabel.ForeColor = Color.Black;
        if (value == 0)
            newLabel.Text = "";
        else newLabel.Text = twoDecimal(value); //String.Format("{0:n2}", value);
        acell.Controls.Add(newLabel);       
        theRow.Cells.Add(acell);
    }

    void createLabelCell(double buyValue, double currentValue, int row, int col, TableRow theRow)
    {
        Label newLabel;
        TableCell acell;

        newLabel = new Label();
        acell = new TableCell();
        newLabel.ID = "row_" + row + "Col_" + col;
        newLabel.Width = 70;
        acell.Width = 70;
        newLabel.Font.Size = FontUnit.Medium;
        if (currentValue< buyValue)
            newLabel.ForeColor = Color.Red;
        else if(currentValue==buyValue)
            newLabel.ForeColor = Color.Black;
        else if(currentValue>buyValue)
            newLabel.ForeColor = Color.Green;
        if (currentValue == 0)
            newLabel.Text = "";
        else newLabel.Text = twoDecimal(currentValue); //String.Format("{0:n2}", value);
        acell.Controls.Add(newLabel);
        theRow.Cells.Add(acell);
    }

    void createCheckBoxCell(int row, int col, TableRow theRow)
    {
        CheckBox newCheckBox;
        TableCell acell;

        newCheckBox = new CheckBox();
        acell = new TableCell();
        newCheckBox.ID = "row_" + row + "Col_" + col;
        newCheckBox.Width = 45;
        acell.Width = 45;
        newCheckBox.Font.Size = FontUnit.Medium;
        acell.Controls.Add(newCheckBox);
        Session[newCheckBox.ID] = newCheckBox.Text;
        theRow.Cells.Add(acell);
    }

    void createHyperLinkCell(string ticker, int row, int col, TableRow theRow)
    {
        TableCell acell;
        HyperLink newLink;
        string idCheck = "";

        acell = new TableCell();
        newLink = new HyperLink();
        newLink.ID = "row_" + row + "Col_" + col;
        newLink.Width = 70;
        acell.Width = 70;
        newLink.Font.Size = FontUnit.Medium;
        newLink.Text = "Analyze";
        newLink.Target = "_Blank";
        newLink.NavigateUrl = "stock.aspx?ticker=" + ticker.ToUpper();
        idCheck = newLink.ID;
        acell.Controls.Add(newLink);
        Session[newLink.ID] = newLink.Text;
        theRow.Cells.Add(acell);
    }

    string twoDecimal(Double number)
    {
        return string.Format("{0:n2}", number);
    }

    string dateFormat(string theDate)
    {
        if (theDate != "")
            return Convert.ToString(Convert.ToDateTime(theDate).ToString("MM/dd/yy"));
        else return "";
    }

    // blank row
    void buildRow(int row)
    {
        TableRow arow = new TableRow();
        int i = 0;

        createCheckBoxCell(row, 0, arow);
        // load the text boxes
        for (i = 1; i < 9; i++) 
            createTextBoxCell("", row, i, arow);

        // load the labels
        for (i = 9; i < 12; i++)
            createLabelCell(0, row, i, arow);

        // Hyperlink
        createHyperLinkCell("", row, 12, arow);
                
        // note
        createTextBoxCell("", row, 13, arow,150);
        portfolioTable.Rows.Add(arow);
    }

    void buildRow(int rowNum, 
                    string ticker,
                    double closePrice)
    {
        TableRow arow = new TableRow();
        int i = 0;

        createCheckBoxCell(rowNum, 0, arow);
        createTextBoxCell(ticker, rowNum, 1, arow);
       
        // load the text boxes
        for (i = 2; i < 8; i++)
            createTextBoxCell("", rowNum, i, arow);

        // load the labels
        createTextBoxCell(closePrice, rowNum, 8, arow);
        for (i = 9; i < 12; i++)
            createLabelCell(0, rowNum, i, arow);

        // Hyperlink
        createHyperLinkCell(ticker, rowNum, 12, arow);
        
        // note
        createTextBoxCell("", rowNum, 13, arow, 150);

        portfolioTable.Rows.Add(arow);       
    }

    void buildRow(int rowNum,
                string ticker,
                int numShares,
                double sharePrice,
                double investment,
                double investmentCost,
                DateTime buyDate,
                DateTime sellDate,
                double closePrice,
                double cashGain,
                double percentGain,
                double positionValue,
                string note)
    {
        string stringBuyDate = Convert.ToString(buyDate.ToString("MM/dd/yy"));
        string stringSellDate = Convert.ToString(sellDate.ToString("MM/dd/yy"));
        buildRow(rowNum, 
                 ticker, 
                 numShares, 
                 sharePrice, 
                 investment, 
                 investmentCost, 
                 stringBuyDate, 
                 stringSellDate, 
                 closePrice,
                 cashGain, 
                 percentGain, 
                 positionValue, 
                 note);
    }

    void buildRow(int rowNum, 
                    string ticker,
                    int numShares,
                    double sharePrice,
                    double investment,
                    double investmentCost,
                    string buyDate, 
                    string sellDate,
                    double closePrice,
                    double cashGain,
                    double percentGain,
                    double positionValue,
                    string note)
    {
        
        TableRow arow = new TableRow();

        createCheckBoxCell(rowNum, 0, arow);

        createTextBoxCell(ticker, rowNum, 1, arow);
        createTextBoxCell(numShares, rowNum, 2, arow);
        createTextBoxCell(sharePrice, rowNum, 3, arow);
        createTextBoxCell(investment, rowNum, 4, arow);
        createTextBoxCell(investmentCost, rowNum, 5, arow);
        createTextBoxCell(buyDate, rowNum, 6, arow);
        createTextBoxCell(sellDate, rowNum, 7, arow);
        createTextBoxCell(closePrice, rowNum, 8, arow);
        
        createLabelCell(cashGain, rowNum, 9, arow);
        createLabelCell(percentGain, rowNum, 10, arow);
        createLabelCell(investment,positionValue, rowNum, 11, arow);
        
        // Hyperlink
        createHyperLinkCell(ticker, rowNum, 12, arow);
        
        // note
        createTextBoxCell(note, rowNum, 13, arow, 150);
        portfolioTable.Rows.Add(arow);
    }

    void buildRow(int rowNum,
                    string ticker,
                    string numShares,
                    string sharePrice,
                    string investment,
                    string investmentCost,
                    string buyDate,
                    string sellDate,
                    string closePrice,
                    string note)
    {

        TableRow arow = new TableRow();

        createCheckBoxCell(rowNum, 0, arow);

        createTextBoxCell(ticker, rowNum, 1, arow);
        createTextBoxCell(numShares, rowNum, 2, arow);
        createTextBoxCell(sharePrice, rowNum, 3, arow);
        createTextBoxCell(investment, rowNum, 4, arow);
        createTextBoxCell(investmentCost, rowNum, 5, arow);
        createTextBoxCell(buyDate, rowNum, 6, arow);
        createTextBoxCell(sellDate, rowNum, 7, arow);
        createTextBoxCell(closePrice, rowNum, 8, arow);

        createLabelCell(0, rowNum, 9, arow);
        createLabelCell(0, rowNum, 10, arow);
        createLabelCell(0, 0, rowNum, 11, arow);

        // Hyperlink
        createHyperLinkCell(ticker, rowNum, 12, arow);

        // note
        createTextBoxCell(note, rowNum, 13, arow, 150);
        portfolioTable.Rows.Add(arow);
    }

    void totals(double sumInvest, double sumValue)
    {
        double sumCashGain = 0;
        double sumPercentGain = 0;

        sumCashGain = sumValue - sumInvest;
        sumPercentGain = ((sumValue - sumInvest) / sumInvest) * 100;

        if (sumPercentGain > 0)
        {
            summaryTable.Rows[1].Cells[0].ForeColor = Color.Green;
            summaryTable.Rows[1].Cells[1].ForeColor = Color.Green;
            summaryTable.Rows[1].Cells[2].ForeColor = Color.Green;
            summaryTable.Rows[1].Cells[3].ForeColor = Color.Green;
        }
        else
        {
            summaryTable.Rows[1].Cells[0].ForeColor = Color.Red;
            summaryTable.Rows[1].Cells[1].ForeColor = Color.Red;
            summaryTable.Rows[1].Cells[2].ForeColor = Color.Red;
            summaryTable.Rows[1].Cells[3].ForeColor = Color.Red;
        }

        if (sumInvest !=0)
            summaryTable.Rows[1].Cells[0].Text = twoDecimal(sumInvest);
        else summaryTable.Rows[1].Cells[0].Text = "";       
        if (sumValue !=0)
            summaryTable.Rows[1].Cells[1].Text = twoDecimal(sumValue);
        else summaryTable.Rows[1].Cells[1].Text = "";
        if(sumCashGain != 0)
            summaryTable.Rows[1].Cells[2].Text = twoDecimal(sumCashGain);
        else summaryTable.Rows[1].Cells[2].Text = "";
        if (sumInvest!=0)
            summaryTable.Rows[1].Cells[3].Text = twoDecimal(sumPercentGain);
        else summaryTable.Rows[1].Cells[3].Text = "";
    }

    // save the portfolio
    protected void portfolioSaveClick(object sender, EventArgs e)
    {
        TableRow arow;
        TextBox dataBox;
        Table theTable = new Table();
        ContentPlaceHolder pageconent;
        SqlCommand mycom = new SqlCommand();
        SqlConnection con = new SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';");
        string insertIntoTable = "";
        string[] positionValues;
        int i = 0;
        int k = 0;
        int sqlResult = 0;

        pageconent = (ContentPlaceHolder)Master.FindControl("MainContent");
        theTable = (Table)pageconent.FindControl("portfolioTable");
        positionValues = new string[9];
        displayPortfolio();
        if (User.Identity.Name != "")
        {
            con.Open();
            mycom.Connection = con;
            mycom.CommandText = "DELETE FROM bobpar.portfolioSaved WHERE (userName = '" + User.Identity.Name + 
                                "') AND (portfolioName = '" + portfolioNameTextBox.Text + "')";
            sqlResult = (int)mycom.ExecuteNonQuery();
            if (portfolioNameTextBox.Text != "")
            {
                mycom.Parameters.Add("@uName", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@pName", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@tick", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@share", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@sPrice", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@invest", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@investCost", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@bDate", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@sDate", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@closePrice", SqlDbType.VarChar, 50);
                mycom.Parameters.Add("@note", SqlDbType.VarChar, 50);
                for (i = 1; i < theTable.Rows.Count-1; i++)
                {
                    arow = new TableRow();
                    arow = (TableRow)theTable.Rows[i];
                    for (k = 0; k < 8; k++)
                    {
                        dataBox = new TextBox();
                        dataBox = (TextBox)arow.Cells[k+1].Controls[0];
                        positionValues[k] = dataBox.Text;
                    }
                    dataBox = new TextBox();
                    dataBox = (TextBox)arow.Cells[13].Controls[0];
                    positionValues[8] = dataBox.Text;
                    insertIntoTable = "INSERT INTO bobpar.portfolioSaved(" + 
                      "userName, portfolioName, ticker, shares, sharePrice, investment, investmentCost, buyDate, sellDate, closePrice, notes) " + 
                    "VALUES (@uName, @pName, @tick, @share, @sPrice, @invest, @investCost, @bDate, @sDate, @closePrice, @note)";
                    mycom.Parameters["@uName"].Value = User.Identity.Name;
                    mycom.Parameters["@pName"].Value = portfolioNameTextBox.Text;
                    mycom.Parameters["@tick"].Value = positionValues[0];
                    mycom.Parameters["@share"].Value = positionValues[1];
                    mycom.Parameters["@sPrice"].Value = positionValues[2];
                    mycom.Parameters["@invest"].Value = positionValues[3];
                    mycom.Parameters["@investCost"].Value = positionValues[4];
                    mycom.Parameters["@bDate"].Value = positionValues[5];
                    mycom.Parameters["@sDate"].Value = positionValues[6];
                    mycom.Parameters["@closePrice"].Value = positionValues[7];
                    mycom.Parameters["@note"].Value = positionValues[8];
                    mycom.CommandText = insertIntoTable;
                    sqlResult = mycom.ExecuteNonQuery();
                    dataStatus.Text = "Last Save: " + DateTime.Now.ToString();
                }
            }
        }
        con.Close();      
        saved_ports();
        portfolioNameTextBox.Text = "";
    }

    // Load portfolio
    protected void portfolioLoadClick(object sender, EventArgs e)
    {
        portfolioLoader(portfolioNameTextBox.Text);       
    }
    
    void portfolioLoader(string portToLoad)
    {
        yahooData stockInfo;

        string ticker = "";
        string insertIntoTable = "";
        string note = "";
        string shares = "";
        string sharePrice = "";
        string investment = "";
        string investmentCost = "";
        string buyDate = "";
        string sellDate = "";
        
        int i = 1;
        int sqlResult = 0;

        string closePrice = "";
        double sumInvest = 0;
        double sumValue = 0;      

        SqlDataReader myreader = null;
        SqlCommand mycom = new SqlCommand();
        SqlConnection con = new SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk;" +
                                                "User ID=dashpar; Password='mathsucks1';");

        portfolioTable.HorizontalAlign = HorizontalAlign.Center;
        portfolioTable.Width = Unit.Percentage(100);
        if (User.Identity.Name != "")
        {
            if (Request.QueryString["port"] != "" && 
                Request.QueryString["port"] != null)
                portfolioNameTextBox.Text = Request.QueryString["port"];       
            mycom.CommandText = "SELECT COUNT(*) FROM bobpar.portfolioSaved WHERE (portfolioName = '"
                    + portToLoad + "') AND (userName = '" + User.Identity.Name + "')";
            con.Open();
            mycom.Connection = con;
            sqlResult = (int)mycom.ExecuteScalar();
            con.Close();
            if (sqlResult > 0)
            {
                if (portToLoad != "")
                {
                    con.Open();
                    insertIntoTable = "SELECT userName, portfolioName, ticker, shares, sharePrice, investment, investmentCost, buyDate, sellDate, closePrice, notes" +
                                        " FROM bobpar.portfolioSaved WHERE (portfolioName = '" +
                                        portToLoad + "') AND (userName = '" + User.Identity.Name + "')";
                    mycom.CommandText = insertIntoTable;
                    myreader = mycom.ExecuteReader();                   
                    while (myreader.Read())
                    {
                        ticker = myreader["ticker"].ToString().TrimEnd();
                        if (myreader["shares"].ToString().TrimEnd() != "")
                            shares = myreader["shares"].ToString().TrimEnd();
                        else shares = "";
                        if (myreader["sharePrice"].ToString().TrimEnd() != "")
                            sharePrice = myreader["sharePrice"].ToString().TrimEnd();
                        else sharePrice = "";
                        if (myreader["investment"].ToString().TrimEnd() != "")
                            investment = myreader["investment"].ToString().TrimEnd();
                        else investment = "";
                        if (myreader["investmentCost"].ToString().TrimEnd() != "")
                            investmentCost = myreader["investmentCost"].ToString().TrimEnd();
                        else investmentCost = "";
                        if (myreader["buyDate"].ToString().TrimEnd() != "")
                            buyDate = myreader["buyDate"].ToString().TrimEnd();
                        else buyDate = "";
                        if (myreader["sellDate"].ToString().TrimEnd() != "")
                            sellDate = myreader["sellDate"].ToString().TrimEnd();
                        else sellDate = "";
                        if (myreader["closePrice"].ToString().TrimEnd() != "")
                            closePrice = myreader["closePrice"].ToString().TrimEnd();
                        else closePrice = "";
                        if (myreader["notes"].ToString().TrimEnd() != "")
                            note = myreader["notes"].ToString().TrimEnd();
                        else note = "";
                        if (sellDate == "")
                        {
                            stockInfo = createYahooData(ticker, buyDate, sellDate);
                            if (stockInfo.close.Count == 0)
                                buildRow(i, ticker, shares, sharePrice, investment, investmentCost,
                                     buyDate, sellDate, closePrice, note);
                            else
                            {
                                closePrice = twoDecimal(stockInfo.close.Last());
                                buildPosition(i, ticker, shares, sharePrice, investment, investmentCost,
                                      buyDate, sellDate, closePrice, note, ref sumInvest, ref sumValue);
                            }
                        }
                        else buildPosition(i, ticker, shares, sharePrice, investment, investmentCost,
                                  buyDate, sellDate, closePrice, note, ref sumInvest, ref sumValue);
                        i++;
                    }
                    buildRow(i);
                }
                con.Close();
            }
           else displayPortfolio();
           saved_ports();
        }
        totals(sumInvest, sumValue);
        portfolioNameTextBox.Text = "";
    }

    yahooData createYahooData(string ticker, string buyDate, string sellDate)
    {
        if (sellDate != "" && buyDate != "")
            return new yahooData(ticker, buyDate, sellDate);
        else if (buyDate != "")
            return new yahooData(ticker, buyDate, DateTime.Today.ToShortDateString());
        else return new yahooData(ticker, DateTime.Today.AddDays(-5));
    }

    private void linkButtonClick(object sender, EventArgs e)
    {
        LinkButton bttn = sender as LinkButton;
        string buttonClicked = bttn.CommandName;
        if (User.Identity.Name == "")
            loadPublicPortfolio(buttonClicked);
        else portfolioLoader(buttonClicked);
    }

    //delete a portfolio
    protected void portfolioDeleteClick(object sender, EventArgs e)
    {
        int sqlResult = 0;

        addRowClick(sender, e);
        SqlCommand mycom = new SqlCommand();
        SqlConnection con = new SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';");
        con.Open();
        mycom.Connection = con;
        mycom.CommandText = "DELETE FROM bobpar.portfolioSaved WHERE (userName = '" + User.Identity.Name +
                            "') AND (portfolioName = '" + portfolioNameTextBox.Text + "')";
        sqlResult = (int)mycom.ExecuteNonQuery();
        con.Close();
        saved_ports();
        portfolioNameTextBox.Text = "";
    }

    // saves the portfolios
    protected void saved_ports()
    {
        int i = 0;
        int j = 0;
        string temp = "";
        TableRow arow = new TableRow();
        TableCell acell = new TableCell();
        List<string> port_names = new List<string>();
        List<string> publicPorts = new List<string>();
        List<string> port_stocks = new List<string>();
        List<int> port_nums = new List<int>();
        SqlDataReader myreader = null;
        SqlCommand mycom = new SqlCommand();
        SqlConnection con = new SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';");

        LinkButton portfolioLButton = new LinkButton();
        CheckBox portfolioCheckBox = new CheckBox();

        savedPortfolioTable.Rows.Clear();
        con.Open();
        mycom.Connection = con;
        mycom.CommandText = "SELECT portfolioName FROM bobpar.portfolioSaved WHERE (userName = '" + User.Identity.Name + "')";
        myreader = mycom.ExecuteReader();
        while (myreader.Read())
        {
            temp = myreader["portfolioName"].ToString().TrimEnd();
            if (port_names.Contains(temp) == false)
            {
                port_names.Add(temp);
                port_stocks.Add(" ");
            }
        }
        con.Close();

        // get the public portfolios
        con.Open();
        mycom.CommandText = "SELECT portfolio FROM bobpar.publicPortfolios WHERE (userName = '" + User.Identity.Name + "')";
        myreader = mycom.ExecuteReader();
        while (myreader.Read())
        {
            temp = myreader["portfolio"].ToString().TrimEnd();
            publicPorts.Add(temp);
        }
        con.Close();

        for (i = 0; i < port_names.Count; i++)
        {
            con.Open();
            mycom.Connection = con;
            mycom.CommandText = "SELECT ticker FROM bobpar.portfolioSaved WHERE (userName = '" + User.Identity.Name + 
                                "') AND (portfolioName = '" + port_names[i] + "')";
            myreader = mycom.ExecuteReader();
            while (myreader.Read())
            {
                if (j < 7)
                    port_stocks[i] += myreader["ticker"].ToString().TrimEnd() + " ";
                j++;
            }
            port_nums.Add(j);
            j = 0;
            con.Close();
        }
        acell = new TableCell();
        arow = new TableRow();

        acell = new TableCell();
        acell.Text = "<center> Public </center>";
        arow.Cells.Add(acell);
        acell = new TableCell();
        acell.Text = "<center> Name </center>";
        arow.Cells.Add(acell);
        acell = new TableCell();
        acell.Text = "<center> Summary </center>";
        arow.Cells.Add(acell);
        acell = new TableCell();
        acell.Text = "<center> Number of Stocks</center>";
        arow.Cells.Add(acell);
        savedPortfolioTable.Rows.Add(arow);
        j = 999;
        for (i = 0; i < port_names.Count; i++)
        {           
            acell = new TableCell();
            arow = new TableRow();
            portfolioCheckBox = new CheckBox();
            portfolioCheckBox.ID = "portfolioCheckBox_" + port_names[i];
            if (publicPorts.Contains(port_names[i]))
                portfolioCheckBox.Checked = true;
            acell.Controls.Add(portfolioCheckBox);
            arow.Cells.Add(acell);

            acell = new TableCell();
            portfolioLButton = new LinkButton();
            portfolioLButton.ID = "portfolioLBbutton"+j;
            portfolioLButton.CommandName = port_names[i];
            portfolioLButton.CommandArgument = "123346";
            portfolioLButton.Text = port_names[i];
            portfolioLButton.Click += new EventHandler(linkButtonClick);
            acell.Controls.Add(portfolioLButton);

            arow.Cells.Add(acell);
            acell = new TableCell();
            acell.Text = "<center>" + port_stocks[i] + "</center>";
            arow.Cells.Add(acell);
            acell = new TableCell();
            acell.Text = "<center>" + port_nums[i].ToString() + "</center>";
            arow.Cells.Add(acell);
            savedPortfolioTable.Rows.Add(arow);
            j++;
        }
    }

    protected void displayPublicPortfolios()
    {
        int i = 0;
        int j = 0;

        SqlDataReader myreader = null;
        SqlCommand mycom = new SqlCommand();
        SqlConnection con = new SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';");
        TableRow arow = new TableRow();
        TableCell acell = new TableCell();
        List<string> port_names = new List<string>();
        List<string> publicPorts = new List<string>();
        List<string> port_stocks = new List<string>();
        List<int> port_nums = new List<int>();

        LinkButton portfolioLButton = new LinkButton();
        CheckBox portfolioCheckBox = new CheckBox();

        string userName = "";
        string portfolioName = "";
        string portStocks = "";

        // get the public portfolios
        con.Open();
        mycom.Connection = con;
        mycom.CommandText = "SELECT userName, Portfolio FROM bobpar.publicPortfolios";
        myreader = mycom.ExecuteReader();
        while (myreader.Read())
        {
            userName = myreader["userName"].ToString().TrimEnd();
            portfolioName = myreader["portfolio"].ToString().TrimEnd();
            publicPorts.Add(userName + '*' + portfolioName);
        }
        con.Close();
        
        for (i = 0; i < publicPorts.Count; i++)
        {
            userName = publicPorts[i].Split('*')[0];
            portfolioName = publicPorts[i].Split('*')[1];
          
            mycom.Connection = con;
            mycom.CommandText = "SELECT ticker FROM bobpar.portfolioSaved WHERE (userName = '" + userName +
                                "') AND (portfolioName = '" + portfolioName + "')";
            con.Open();
            myreader = mycom.ExecuteReader();
            portStocks = "";
            while (myreader.Read())
            {
                if (j < 7)
                    portStocks += myreader["ticker"].ToString().TrimEnd() + " ";
                j++;
            }
            port_stocks.Add(portStocks);
            port_nums.Add(j);
            j = 0;
            con.Close();
        }
        acell = new TableCell();
        arow = new TableRow();

        acell = new TableCell();
        acell.Text = "<center> User Name </center>";
        arow.Cells.Add(acell);

        acell = new TableCell();
        acell.Text = "<center> Portfolio Name </center>";
        arow.Cells.Add(acell);
        acell = new TableCell();
        acell.Text = "<center> Summary </center>";
        arow.Cells.Add(acell);
        acell = new TableCell();
        acell.Text = "<center> Number of Stocks</center>";
        arow.Cells.Add(acell);
        publicTable.Rows.Add(arow);
        j = 999;
        for (i = 0; i < publicPorts.Count; i++)
        {
            acell = new TableCell();
            arow = new TableRow();

            acell.Text = publicPorts[i].Split('*')[0];
            arow.Cells.Add(acell);

            acell = new TableCell();
            portfolioLButton = new LinkButton();
            portfolioLButton.ID = "portfolioLBbutton" + j;
            portfolioLButton.CommandName = publicPorts[i];
            portfolioLButton.CommandArgument = "123346";
            portfolioLButton.Text = publicPorts[i].Split('*')[1];
            portfolioLButton.Click += new EventHandler(linkButtonClick);
            acell.Controls.Add(portfolioLButton);

            arow.Cells.Add(acell);
            acell = new TableCell();
            acell.Text = "<center>" + port_stocks[i] + "</center>";
            arow.Cells.Add(acell);
            acell = new TableCell();
            acell.Text = "<center>" + port_nums[i].ToString() + "</center>";
            arow.Cells.Add(acell);
            publicTable.Rows.Add(arow);
            j++;
        }
    }
    void loadPublicPortfolio(string portToLoad)
    {
        yahooData stockInfo;

        string ticker = "";
        string insertIntoTable = "";
        string note = "";
        string shares = "";
        string sharePrice = "";
        string investment = "";
        string investmentCost = "";
        string buyDate = "";
        string sellDate = "";

        int i = 1;
        int sqlResult = 0;

        string closePrice = "";
        double sumInvest = 0;
        double sumValue = 0;

        SqlDataReader myreader = null;
        SqlCommand mycom = new SqlCommand();
        SqlConnection con = new SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk;" +
                                                "User ID=dashpar; Password='mathsucks1';");

        portfolioTable.HorizontalAlign = HorizontalAlign.Center;
        portfolioTable.Width = Unit.Percentage(100);
        if (Request.QueryString["port"] != "" &&
            Request.QueryString["port"] != null)
            portfolioNameTextBox.Text = Request.QueryString["port"];
        
        mycom.Connection = con;
        con.Open();
        mycom.CommandText = "SELECT COUNT(*) FROM bobpar.portfolioSaved WHERE (portfolioName = '"
                + portToLoad.Split('*')[1] + "') AND (userName = '" + portToLoad.Split('*')[0] + "')";

        sqlResult = (int)mycom.ExecuteScalar();
        con.Close();
        
        if (sqlResult > 0)
        {
            if (portToLoad != "")
            {
                con.Open();
                insertIntoTable = "SELECT userName, portfolioName, ticker, shares, sharePrice, investment, investmentCost, buyDate, sellDate, closePrice, notes" +
                                    " FROM bobpar.portfolioSaved WHERE (portfolioName = '" +
                                    portToLoad.Split('*')[1] + "') AND (userName = '" + portToLoad.Split('*')[0] + "')";
                mycom.CommandText = insertIntoTable;
                myreader = mycom.ExecuteReader();
                while (myreader.Read())
                {
                    ticker = myreader["ticker"].ToString().TrimEnd();
                    if (myreader["shares"].ToString().TrimEnd() != "")
                        shares = myreader["shares"].ToString().TrimEnd();
                    else shares = "";
                    if (myreader["sharePrice"].ToString().TrimEnd() != "")
                        sharePrice = myreader["sharePrice"].ToString().TrimEnd();
                    else sharePrice = "";
                    if (myreader["investment"].ToString().TrimEnd() != "")
                        investment = myreader["investment"].ToString().TrimEnd();
                    else investment = "";
                    if (myreader["investmentCost"].ToString().TrimEnd() != "")
                        investmentCost = myreader["investmentCost"].ToString().TrimEnd();
                    else investmentCost = "";
                    if (myreader["buyDate"].ToString().TrimEnd() != "")
                        buyDate = myreader["buyDate"].ToString().TrimEnd();
                    else buyDate = "";
                    if (myreader["sellDate"].ToString().TrimEnd() != "")
                        sellDate = myreader["sellDate"].ToString().TrimEnd();
                    else sellDate = "";
                    if (myreader["closePrice"].ToString().TrimEnd() != "")
                        closePrice = myreader["closePrice"].ToString().TrimEnd();
                    else closePrice = "";
                    if (myreader["notes"].ToString().TrimEnd() != "")
                        note = myreader["notes"].ToString().TrimEnd();
                    else note = "";
                    if (sellDate == "")
                    {
                        stockInfo = createYahooData(ticker, buyDate, sellDate);
                        if (stockInfo.close.Count == 0)
                            buildRow(i, ticker, shares, sharePrice, investment, investmentCost,
                                    buyDate, sellDate, closePrice, note);
                        else
                        {
                            closePrice = twoDecimal(stockInfo.close.Last());
                            buildPosition(i, ticker, shares, sharePrice, investment, investmentCost,
                                    buyDate, sellDate, closePrice, note, ref sumInvest, ref sumValue);
                        }
                    }
                    else buildPosition(i, ticker, shares, sharePrice, investment, investmentCost,
                                buyDate, sellDate, closePrice, note, ref sumInvest, ref sumValue);
                    i++;
                }
                buildRow(i);
            }
            con.Close();
        }
        //else displayPortfolio();
        
        //displayPublicPortfolios();
        totals(sumInvest, sumValue);
    }
    

}
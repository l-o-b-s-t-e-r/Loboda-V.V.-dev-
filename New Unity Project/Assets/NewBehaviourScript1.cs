using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Line //Класс для определения свойств вектора (длина, угол и в зависимости от угла номер от 1-4 или (-1) до (-4))
{
    public Vector2 v1, v2;
    public int chetvert;
    private int num;
    public float angle;

    public Line(Vector2 v1, Vector2 v2)
    {
        this.v1 = v1;
        this.v2 = v2;
        getAngle();
    }

    public Line(Line l)
    {
        this.v1 = l.v1;
        this.v2 = l.v2;
        getAngle();
    }

    public int getNum()
    {
        return num;
    }

    public float length(){
        float kat1, kat2;
        kat1 = Mathf.Abs(v1.x - v2.x);
        kat2 = Mathf.Abs(v1.y - v2.y);

        return Mathf.Sqrt(kat1*kat1+kat2*kat2);
    }
    public float getAngle()
    {
        if ((v1.x < v2.x && v1.y < v2.y) || (v1.x > v2.x && v1.y > v2.y)) chetvert = 1;
        if ((v1.x > v2.x && v1.y < v2.y) || (v1.x < v2.x && v1.y > v2.y)) chetvert = 2;
        if (v1.x == v2.x) chetvert = 3;
        if (v1.y == v2.y) chetvert = 4;

        if (chetvert == 1)
        {
            angle = Mathf.Acos((Mathf.Abs(v1.x - v2.x) / length())) * 57.0f;
            if (v1.x < v2.x && v1.y < v2.y){
                if (angle > 0.0f && angle < 15.0f) num = 2;
                if (angle >= 10.0f && angle <= 75.0f) num = 1;
                if (angle > 75.0f && angle < 90.0f) num = -4;
            }
            if (v1.x > v2.x && v1.y > v2.y)
            {
                if (angle > 0.0f && angle < 15.0f) num = -2;
                if (angle >= 15.0f && angle <= 75.0f) num = -1;
                if (angle > 75.0f && angle < 90.0f) num = 4;
            }
        }

        if (chetvert == 2)
        {
            angle = 90.0f - Mathf.Acos((Mathf.Abs(v1.x - v2.x) / length())) * 57.0f;
            if (v1.x > v2.x && v1.y < v2.y)
            {
                if (angle > 0.0f && angle < 15.0f) num = -4;
                if (angle >= 15.0f && angle <= 75.0f) num = -3;
                if (angle > 75.0f && angle < 90.0f) num = -2;
            }
            if (v1.x < v2.x && v1.y > v2.y)
            {
                if (angle > 0.0f && angle < 15.0f) num = 4;
                if (angle >= 15.0f && angle <= 75.0f) num = 3;
                if (angle > 75.0f && angle < 90.0f) num = 2;
            }
        }

        if (chetvert == 3)
        {
            if (v1.y < v2.y) num = -4;
               else num = 4;
        }

        if (chetvert == 4)
        {
            if (v1.x < v2.x) num = 2;
            else num = -2;
        }

        return 0.0f;
    }
}

public class Figure //класс который содержит в себе шаблон фигуры 
                    //(напряавления сторон и отношение каждой стороны к общей длине сторон)
{
    public int[] lineSides;
    public float[] lengthSides;
    private int countSides;

    public Figure(int[] line, float[] length, int n)
    {
        countSides = n;
        lineSides = new int[countSides];
        lengthSides = new float[countSides];

        for (int i = 0; i < countSides; i++)
        {
            lineSides[i] = line[i];
            lengthSides[i] = length[i];
        }
    }


    public int getCountSides(){
        return countSides;
    }

}


public class NewBehaviourScript1 : MonoBehaviour
{
    public float time = 5.0f; //время игры в секундах
    public float alpha = 0.1f; //величина допустимой погрешности

    public Button btnNext2, btnPlay, bntAgain, btnAdd;
    public CanvasGroup cnvsTriangle, cnvsСircle, cnvsSquare, cnvsRule;
    public InputField edit;
    public Text txt, txtLevel, txtFigName;
    public GameObject obj4;
    public LineRenderer lineRenderer;
    
    Vector2 pos1, pos2 = new Vector2(); //первая и вторая точки вектора
    int count = 0, choose = 0, level, countFigures = 0, numDraw = 0; //кол-во точек, номер фигуры, уровень
    float Xnew = 0.0f, Xold = 0.0f, Ynew = 0.0f, Yold = 0.0f; //коодинаты новой точки и координаты предыдущей точки
    float SumLength,lastTime, currentTime=1.0f; //какое было время когда последний раз срабатывала процедура Update, текущее время
    bool adding=false,flag = false,firstTime = false, firstPoint = false, secondPoint = false, play = false, onMouseUp = false, timeFlag = false, end = false;

    string[] NameOfFigures = new string[10];
    Line[] lines = new Line[500]; //последовательность векторов фигуры
    Line[][] newFigureLine = new Line[5][]; //при добавлении новой фигуры, все 5 тестовых экземлярова заносятся в этот массив
    Figure[] figure = new Figure[10]; //содержит шаблоны фигур

    Color[] cl = { Color.yellow, Color.red, Color.green, Color.magenta, Color.blue };
    //шаблоны фигур
    float[] lengths = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }; //длины сторон
    
    int[] triangle1 ={ -1, 2, -3 };
    float[] triangleSides = { 0.35f, 0.3f, 0.35f };
    int[] circle1 = { 1, 2, 3, 4, -1, -2, -3, -4};
    float[] circleSides = { 1.0f / 8.0f, 1.0f / 8.0f, 1.0f / 8.0f, 1.0f / 8.0f, 1.0f / 8.0f, 1.0f / 8.0f, 1.0f / 8.0f, 1.0f / 8.0f};
    int[] square1 = { 4, 2, -4, -2 };
    float[] SquareSides = {0.25f, 0.25f, 0.25f, 0.25f};

    void choiceFigure() //Генерируется фигура которую нужно будет нарисовать
    {
        int tmp;
        while ((tmp = Random.Range(0, countFigures)) == choose) { };
        choose = tmp;
        switch (choose)
        {
            case 0: cnvsTriangle.active = true; cnvsСircle.active = false; cnvsSquare.active = false; txtFigName.active = false; break;
            case 1: cnvsСircle.active = true; cnvsTriangle.active = false; cnvsSquare.active = false; txtFigName.active = false; break;
            case 2: cnvsSquare.active = true; cnvsTriangle.active = false; cnvsСircle.active = false; txtFigName.active = false; break;
            default: cnvsTriangle.active =false; cnvsСircle.active = false; cnvsSquare.active = false; 
                     txtFigName.active=true; txtFigName.text = NameOfFigures[choose]; break;
        }
    }

    void inicializNext() //Инициализации полей, если программа распознала фигуру верно
    {
        btnNext2.active = true;
        play = onMouseUp = timeFlag = flag = false;
        time = currentTime;
    }

    void inicializAgain() //Инициализация полей, если фигура была распознана не верно
    {
        txt.text = "0.00";

        bntAgain.active = end = true;
        play = onMouseUp = timeFlag = flag = false;
    }

    public void buttonAdd()
    {
        btnPlay.active = false;
        btnAdd.active = false;
        cnvsRule.active = true;
        adding = true;
        play = true;
    }

    public void buttonPlay()
    {
        level = 1;
        btnPlay.active = false;
        btnAdd.active = false;
        txt.active = true;
        txtLevel.active = true;
        txtLevel.text = "Level " + level;
        level++;
        play = true;
        choiceFigure();
    }

    public void buttonAgain() //Кнопка для начала новой игры
    {
        NameOfFigures[countFigures-1] = edit.text;
        btnPlay.active = true;
        btnAdd.active = true;
        edit.active = false;
        txt.active = false;
        bntAgain.active = false;
        txtLevel.active = false;
        cnvsTriangle.active = false;
        cnvsSquare.active = false;
        cnvsСircle.active = false;

        count = 0;
        lastTime = 0.0f;
        time =60.0f;
        lineRenderer.SetColors(Color.black, Color.black);
        
    }

    public void buttinNext()
    {
        btnNext2.active = false;
        count = 0;
        lineRenderer.SetColors(Color.black, Color.black);
        play = true;
        end = false;
        lastTime = Time.time;
        txtLevel.text = "Level " + level;
        level++;
        choiceFigure();
    }

    void Start() 
    {
        figure[countFigures] = new Figure(triangle1, triangleSides, triangle1.Length);
        NameOfFigures[countFigures] = "triangle";
        countFigures++;
        figure[countFigures] = new Figure(circle1, circleSides, circle1.Length);
        NameOfFigures[countFigures] = "circle";
        countFigures++;
        figure[countFigures] = new Figure(square1, SquareSides, square1.Length);
        NameOfFigures[countFigures] = "square";
        countFigures++;
    }


    bool YesOrNo(Figure mass, int[] line, int count) //Идет проверка нарисованной фигуры с эталоном, который задан изначально
    { 
        int j = 0, tmp;
        if (mass.getCountSides() != count)
        {  return false; }

        
        for (int m = 1; m <= 2; m++)
            {
            for (int i = 0; i < mass.getCountSides(); i++)
            {

                int k = 0;
                for (j = 0; j < mass.getCountSides(); j++)
                    if (mass.lineSides[j] == line[j]) k++;
                if (k == mass.getCountSides()) { return true; }
                else
                {
                    tmp = mass.lineSides[0];
                    for (j = 0; j < mass.getCountSides() - 1; j++)
                        mass.lineSides[j] = mass.lineSides[j + 1];

                    mass.lineSides[mass.getCountSides() - 1] = tmp;
                }
            }

            for (int i = 0; i < figure[choose].getCountSides() / 2; i++)
            {
                int tm = figure[choose].lineSides[i];
                figure[choose].lineSides[i] = -1 * figure[choose].lineSides[figure[choose].getCountSides() - 1 - i];
                figure[choose].lineSides[figure[choose].getCountSides() - 1 - i] = -1 * tm;
            }
            if (figure[choose].getCountSides() % 2 != 0) figure[choose].lineSides[figure[choose].getCountSides() / 2] *= -1;
        }

        return false;
    }


    void anomValueStep1() //поиск аномальных векторов в фигуре Шаг1
    {
        float Sum1 = 0.0f, Sum2 = 0.0f;

        for (int j = 0; j < lengths.Length / 2; j++)
        {
            lengths[j] = 0.0f;
            for (int i = 0; i < count - 1; i++)
                if (lines[i].getNum() == j + 1)
                    lengths[j] += lines[i].length();
        }

        for (int j = 0; j > -lengths.Length / 2; j--)
        {
            lengths[4 + Mathf.Abs(j)] = 0.0f;
            for (int i = 0; i < count - 1; i++)
                if (lines[i].getNum() == j - 1)
                    lengths[4 + Mathf.Abs(j)] += lines[i].length();
        }

        SumLength = 0.0f;
        for (int i = 0; i < lengths.Length; i++)
            SumLength += lengths[i];

        //--------------------------------------------------------------------
        for (int i = 0; i < figure[choose].getCountSides(); i++)
            for (int j = 1; j <= lengths.Length; j++)
            {
                if (figure[choose].lineSides[i] < 0 && Mathf.Abs(figure[choose].lineSides[i]) + 4 == j) Sum1 += lengths[j - 1];
                if (figure[choose].lineSides[i] > 0 && figure[choose].lineSides[i] == j) Sum1 += lengths[j - 1];
            }

        for (int i = 0; i < figure[choose].getCountSides()/2; i++){
            int tmp = figure[choose].lineSides[i];
            figure[choose].lineSides[i]=-1*figure[choose].lineSides[figure[choose].getCountSides() - 1 - i];
            figure[choose].lineSides[figure[choose].getCountSides() - 1 - i]=-1*tmp;
            }
        if (figure[choose].getCountSides() % 2 != 0) figure[choose].lineSides[figure[choose].getCountSides()/2] *= -1;


        for (int i = 0; i < figure[choose].getCountSides(); i++)
            for (int j = 1; j <= lengths.Length; j++)
            {
                if (figure[choose].lineSides[i] < 0 && Mathf.Abs(figure[choose].lineSides[i]) + 4 == j) Sum2 += lengths[j - 1];
                if (figure[choose].lineSides[i] > 0 && figure[choose].lineSides[i] == j) Sum2 += lengths[j - 1];
            }

        if (Sum1 > Sum2)
            for (int i = 0; i < figure[choose].getCountSides() / 2; i++)
            {
                int tmp = figure[choose].lineSides[i];
                figure[choose].lineSides[i] = -1 * figure[choose].lineSides[figure[choose].getCountSides() - 1 - i];
                figure[choose].lineSides[figure[choose].getCountSides() - 1 - i] = -1 * tmp;
            }
            if (figure[choose].getCountSides() % 2 != 0) figure[choose].lineSides[figure[choose].getCountSides() / 2] *= -1;
        //-----------------------------------------------------------------------

    }
    
    int anomValue() //возвращает аном. значение
    {
        for (int j = 1; j <= lengths.Length; j++)
        {
            bool f = false;
            for (int i = 0; i < figure[choose].getCountSides(); i++)
            {
                if (figure[choose].lineSides[i] < 0 && Mathf.Abs(figure[choose].lineSides[i]) + 4 == j)
                { i = figure[choose].getCountSides(); f = true; }
                else 
                    if (figure[choose].lineSides[i] > 0 && figure[choose].lineSides[i] == j)
                    { i = figure[choose].getCountSides(); f = true; }
            }
            if (f == false && lengths[j - 1] > 0.0f && lengths[j - 1] < alpha * SumLength) {  lengths[j - 1] = 0.0f; return j; }
        }

        return 0;
    }


    bool lengthOfSides(Figure mass, float[] newLengths) //Идет проверка на соотношения сторон
    {
            int k = 0;
            for (int i = 0; i < mass.getCountSides(); i++)   
                if (newLengths[i] / SumLength > mass.lengthSides[i] - alpha && newLengths[i] / SumLength < mass.lengthSides[i] + alpha) 
                    k++;

        if (k == mass.getCountSides()) return true;

        return false;
    }

    void anomValueStep2(Line[] mass)
    {
        int k = 0;
        while ((k = anomValue()) != 0)
            for (int i = 0; i < count - 1; i++)
                if ((mass[i].getNum() > 0 && mass[i].getNum() == k) ||
                    (mass[i].getNum() < 0 && (Mathf.Abs(mass[i].getNum()) + 4) == k))
                {
                    for (int j = i + 1; j < count - 1; j++)
                        mass[j - 1] = mass[j];

                    count--;
                    i--;
                }
    }

    void anomValueStep3(Line[] mass)
    {
        for (int i = 1; i < count - 1; i++)
            if (mass[i - 1].getNum() != mass[i].getNum() && mass[i - 1].getNum() != mass[i + 1].getNum())
            {
                for (int j = i; j < count - 1; j++)
                    mass[j - 1] = mass[j];

                count--;
            }
  
    }

    void endDrawing() 
    {
                    anomValueStep1();
                    anomValueStep2(lines);
                    anomValueStep3(lines);

                   
                    int[] lineNum = new int[100];
                    float[] newLengths = new float[100];
                    int lineNumCount = 0;
                    lineNum[lineNumCount] = lines[0].getNum();
                    newLengths[lineNumCount] = lines[0].length();
                    lineNumCount++;


                    for (int i = 1; i < count - 1; i++)
                        if (lines[i].getNum() != lines[i - 1].getNum())
                        {                             
                            lineNum[lineNumCount] = lines[i].getNum();
                            newLengths[lineNumCount] = lines[i].length();
                            lineNumCount++; 
                        }
                        else newLengths[lineNumCount - 1] += lines[i].length();


                    if (lineNum[0] == lineNum[lineNumCount - 1])
                    {
                        newLengths[0] += newLengths[lineNumCount - 1];
                        lineNumCount--;
                    }


                        if (YesOrNo(figure[choose], lineNum, lineNumCount) && lengthOfSides(figure[choose],newLengths))
                        { inicializNext(); Debug.Log("YES!"); }
                        else
                        {
                            Debug.Log("NO!");
                            count = 0;
                            lineRenderer.SetColors(Color.black, Color.black);
                            end = onMouseUp = flag = false;
                            play = true;
                        }

    }

    bool linesAreCross(Line line1, Line line2, int n)
    {

        if (Mathf.Max(line2.v1.x, line2.v2.x) > Mathf.Min(line1.v1.x, line1.v2.x) && Mathf.Min(line2.v2.x, line2.v1.x) < Mathf.Max(line1.v1.x, line1.v2.x) &&
            Mathf.Max(line2.v1.y, line2.v2.y) > Mathf.Min(line1.v1.y, line1.v2.y) && Mathf.Min(line2.v2.y, line2.v1.y) < Mathf.Max(line1.v1.y, line1.v2.y))
            {
              count -= n+1;
              for (int i = 0; i < count; i++)
              {
                  lines[i] = lines[i + n];
                  lineRenderer.SetVertexCount(i + 1);
                  lineRenderer.SetPosition(i, lines[i].v1);
              }

                  return true; 
            }
        else return false;
    }

    void drawNewFigure(){
        numDraw++;

        newFigureLine[numDraw - 1] = new Line[count - 1];
        for (int i = 0; i < count - 1; i++)
            newFigureLine[numDraw - 1][i] = new Line(lines[i]);

        if (numDraw == 5) 
        { 
          numDraw = 0; 
          adding = false;
          edit.active = true;
          cnvsRule.active = false;
          inicializAgain();
          addFigure();
        }
        else
        {
            count = 0;
            lineRenderer.SetColors(Color.black, Color.black);
            end = onMouseUp = timeFlag = flag = false;
            play = true;
        }
    }

    void addFigure() //добавление новой фигуры
    {
        int[] n = {0,0,0,0,0};
        for (int i=0; i<newFigureLine.Length; i++) //определяем фигуру которая наиболее похожа на все остальные
            for (int j=0; j<newFigureLine.Length; j++)
                if (i != j)
                    for (int k = 0; k < Mathf.Min(newFigureLine[i].Length, newFigureLine[j].Length); k++)
                         if (newFigureLine[i][k].getNum() == newFigureLine[j][k].getNum()) n[i]++;

        int n1 = Mathf.Max(n[0],n[1],n[2],n[3],n[4]);
        int numFigure=0;
        for (int i = 0; i < 5; i++)
            if (n1 == n[i]) { numFigure = i; i = 5; }

        count = newFigureLine[numFigure].Length + 1;

        int m = 0;
        while ((m = AnomNewFig(newFigureLine[numFigure])) != 0) // начинается процедуру получения шаблона (удление ан. значений ...)
            for (int i = 0; i < count - 1; i++)
                if ((newFigureLine[numFigure][i].getNum() > 0 && newFigureLine[numFigure][i].getNum() == m) || 
                    (newFigureLine[numFigure][i].getNum() < 0 && (Mathf.Abs(newFigureLine[numFigure][i].getNum()) + 4) == m))
                {
                    for (int j = i + 1; j < count - 1; j++)
                        newFigureLine[numFigure][j - 1] = newFigureLine[numFigure][j];

                    count--;
                    i--;
                }

        anomValueStep3(newFigureLine[numFigure]);

        int[] lineNum = new int[100];
        int lineNumCount = 0;
        lineNum[lineNumCount] = newFigureLine[numFigure][0].getNum();
        lineNumCount++;

        for (int i = 1; i < count - 1; i++)
            if (newFigureLine[numFigure][i].getNum() != newFigureLine[numFigure][i - 1].getNum())
            { lineNum[lineNumCount] = newFigureLine[numFigure][i].getNum(); lineNumCount++; }


        if (lineNum[0] == lineNum[lineNumCount - 1]) lineNumCount--;

        float[] NewsLengths = new float[lineNumCount];
        SumLength = 0.0f;
        for (int i = 0; i < NewsLengths.Length; i++)
            if (lineNum[i] < 0) SumLength+= lengths[Mathf.Abs(lineNum[i]) + 3];
            else SumLength += lengths[lineNum[i]-1];

        for (int i = 0; i < NewsLengths.Length; i++)
             if (lineNum[i] < 0) NewsLengths[i] = lengths[Mathf.Abs(lineNum[i]) + 3]/SumLength;
             else NewsLengths[i] = lengths[lineNum[i]-1]/SumLength;

        figure[countFigures] = new Figure(lineNum, NewsLengths, lineNumCount); //добавляем полученный шаблон 
        countFigures++;                    

    }

    int AnomNewFig(Line[] mass)
    {
        for (int j = 0; j < lengths.Length / 2; j++)
        {
            lengths[j] = 0.0f;
            for (int i = 0; i < count - 1; i++)
                if (mass[i].getNum() == j + 1)
                    lengths[j] += mass[i].length();
        }

        for (int j = 0; j > -lengths.Length / 2; j--)
        {
            lengths[4 + Mathf.Abs(j)] = 0.0f;
            for (int i = 0; i < count - 1; i++)
                if (mass[i].getNum() == j - 1)
                    lengths[4 + Mathf.Abs(j)] += mass[i].length();
        }

        SumLength = 0.0f;
        for (int i = 0; i < lengths.Length; i++)
            SumLength += lengths[i];


        for (int i = 0; i < lengths.Length; i++)
            if (lengths[i] < SumLength * alpha && lengths[i] > 0)
                return i + 1;

        return 0;
    }


    void Update()
    {
        if (timeFlag == false) lastTime = Time.time;
        if (adding == true) lastTime = Time.time;
        if (play == true && (currentTime = time - (Time.time - lastTime))>0.0)
        {
            if (timeFlag == false) {lastTime = Time.time; timeFlag = true; }
            txt.text = currentTime.ToString("0.00");
            if (flag == true)
            {
                Xnew = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                Ynew = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

                if ((Xnew != Xold || Ynew != Yold) && (new Line(new Vector2(Xnew, Ynew), new Vector2(Xold, Yold))).length() > 0.1)    {
                    Xold = Xnew;
                    Yold = Ynew;
                    if (firstTime == true)
                    {
                        int a, b;
                        a = Random.Range(0, 5);
                        while ((b = Random.Range(0, 5)) == a) { }

                        lineRenderer.SetColors(cl[a], cl[b]);
                        lineRenderer.SetVertexCount(count + 1);
                        obj4.transform.position = new Vector3(pos1.x, pos1.y, 0.0f);
                        lineRenderer.SetPosition(count, pos1); count++;

                    }

                    if (firstPoint == true)
                    {
                        pos1.x = Xold;
                        pos1.y = Yold;

                        lineRenderer.SetVertexCount(count + 1);
                        lineRenderer.SetPosition(count, pos1);
                        obj4.transform.position = new Vector3(pos1.x, pos1.y, 0.0f);
                        lines[count - 1] = new Line(pos2, pos1);
                        count++;
                        for (int i = 0; count > 2 && i < count-5; i++)
                            if (linesAreCross(lines[count - 2], lines[i],i))
                            {
                                if (adding == false) endDrawing();
                                else drawNewFigure();
                            }

                            firstPoint = false;
                    }
                    else
                    {
                        pos2.x = Xold;
                        pos2.y = Yold;
                     
                        lineRenderer.SetVertexCount(count + 1);
                        lineRenderer.SetPosition(count, pos2);
                        obj4.transform.position = new Vector3(pos2.x, pos2.y, 0.0f);
                        lines[count - 1] = new Line(pos1, pos2);
                        count++;
                        for (int i = 0; count > 2 && i < count-5; i++)
                            if (linesAreCross(lines[count - 2], lines[i],i))
                            {
                                if (adding == false) endDrawing();
                                else drawNewFigure();
                            }

                        firstPoint = true;
                        firstTime = false;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    flag = true;
                    firstTime = true;
                    firstPoint = false;
                    onMouseUp = true;
                    pos1.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                    pos1.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

                    Xold = pos1.x;
                    Yold = pos1.y;
                }
            }
            if (Input.GetMouseButtonUp(0) && onMouseUp == true)
                if (adding == false) endDrawing();
                else drawNewFigure();
        
        }
        else       
            if (currentTime <= 0.0 && end==false)         
                inicializAgain();
        
        }
    }


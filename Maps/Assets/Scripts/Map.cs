﻿using System;
using UnityEngine;
using System.Collections;
using System.Net;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.GoogleMaps.Scripts;
using System.Text;

public class Map : MonoBehaviour
{
    public enum MapType
    {
        RoadMap,
        Satellite,
        Terrain,
        Hybrid
    }
    public bool loadOnStart;
    public bool autoLocateCenter;
    public GoogleMapLocation centerLocation;
    public int zoom;
    public MapType mapType;
    public int size;
    public bool doubleResolution;
    public List<GoogleMapMarker> markers;

    public GameObject panel;

    private string url = "http://maps.googleapis.com/maps/api/staticmap";

    void Start()
    {
        HTTP.Request.proxy = new Uri("http://217.23.121.11:3128");

        if (loadOnStart) Refresh();
    }
    public void Refresh()
    {
        if (autoLocateCenter && (markers.Count == 0))
        {
            Debug.LogError("Auto Center will only work if paths or markers are used.");
        }
        StartCoroutine(Process());
    }

    //Последовательность
    //Первым делом берем нужный участок голый (без маршрутов)
    //Далее запрос той же местности + координаты участка (N раз), тоесть имею N+1 изображение
    //Далее сравниваю все N изображений по пиксельно с исходным, рисую карту разниц, типа если не совпало тикущее с исходным, значит на результирующем надо отрисовать


    public GoogleMapPath GetPathFromString(string coords)
    {
        var p = new GoogleMapPath();
        try
        {
            coords = coords.Trim();
            if (coords[coords.Length - 1] == ';') coords = coords.Remove(coords.Length - 1);

            p.color = GoogleMapColor.red;

            var locat = new List<GoogleMapLocation>();
            foreach (var c in coords.Split(';'))
            {
                var l = new GoogleMapLocation();
                try
                {
                    l.latitude = float.Parse(c.Split(',')[0]);
                    l.longitude = float.Parse(c.Split(',')[1]);
                    locat.Add(l);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message + "=" + c);
                    throw;
                }
            }
            p.locations = locat;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
        return p;
    }

    public bool HaveEmpty(Texture2D[] pathTextures)
    {
        foreach (var p in pathTextures) if (p == null) return true;
        return false;
    }
    public IEnumerator Process()
    {
        Texture2D baseTexture = null;
        int threadId = 0;
        StartCoroutine(GetImage(threadId++, null, 0, (x) => baseTexture = x.Texture));

        List<string> paths = new List<string>();
        paths.Add("53.84264020414466, 27.396538853645325;53.91690820601384, 27.603841423988342;53.918386786675605, 27.603766322135925;53.91968208140718, 27.602832913398743;53.92227255031691, 27.601073384284973;53.92327078567368, 27.59991466999054;53.92454697585717, 27.598584294319153;53.9259747463536, 27.594056725502014;53.92721926752343, 27.589990496635437;53.92827423864236, 27.58646070957184;53.92944920502722, 27.58267343044281;53.930680989107415, 27.578371167182922;53.93039041767784, 27.577266097068787;53.93016301253918, 27.577105164527893;53.93013774522507, 27.57621467113495;53.92809104197802, 27.573886513710022;53.926088460488145, 27.57192313671112;53.92404155866841, 27.570067048072815;53.92133115459852, 27.56794273853302;53.91879749423395, 27.565636038780212;53.91677551031722, 27.564101815223694;53.91480398172081, 27.5624817609787;53.912939790540534, 27.561001181602478;53.910456179448985, 27.55899488925934;53.908857237162934, 27.55766451358795;53.90693589748128, 27.55618393421173;53.905785579455625, 27.554767727851868;53.905166164320605, 27.55370557308197;53.904660512503085, 27.552761435508728;53.90498286624398, 27.5525039434433;53.90461626767786, 27.552160620689392;53.9038514568639, 27.55119502544403;53.90191724472071, 27.548405528068542;53.900741503382605, 27.54693567752838;53.90026740477252, 27.54625976085663;53.899591014776156, 27.544779181480408;53.89904736628095, 27.54323422908783;53.897738787991734, 27.540412545204163;53.89696120756826, 27.538556456565857;53.89706867889506, 27.53769814968109;53.896986494964146, 27.537140250205994;53.89574107253449, 27.53651797771454;53.894799078630896, 27.53369629383087;53.89365474874657, 27.529587149620056;53.89283915749413, 27.527838349342346;53.89170741923439, 27.525617480278015;53.89046816230106, 27.523428797721863;53.88927312974328, 27.521079182624817;53.88777454856646, 27.517367005348206;53.88707266295829, 27.515199780464172;53.887205453031335, 27.5145024061203;53.88712957304123, 27.513901591300964;53.88658576241459, 27.513858675956726;53.885415913250675, 27.51080095767975;53.8850934085353, 27.509331107139587;53.88391719378349, 27.506069540977478;53.88400572722838, 27.505179047584534;53.88379071710842, 27.50503957271576;53.88339863698405, 27.50403106212616;53.88303817298023, 27.503677010536194;53.88259549363577, 27.502593398094177;53.88205794813009, 27.501445412635803;53.88164687925638, 27.500898241996765;53.881052403274865, 27.50024378299713;53.880059483052605, 27.49904215335846;53.87944601100317, 27.498355507850647;53.87883885454552, 27.497937083244324;53.877940752786486, 27.49729335308075;53.87713750459046, 27.49679982662201;53.876523989669614, 27.496477961540222;53.87511982886247, 27.49552309513092;53.87429755064501, 27.494997382164;53.87306410300353, 27.494085431098938;53.87270987541868, 27.493924498558044;53.87251378385836, 27.492433190345764;53.872425226078136, 27.491349577903748;53.8722354587748, 27.490169405937195;53.87245052832018, 27.488495707511902;53.87234299368575, 27.486714720726013;53.87200141124817, 27.484354376792908;53.87195713183904, 27.48228371143341;53.87195713183904, 27.47982680797577;53.87193815493508, 27.478442788124084;53.87191917802248, 27.475857138633728;53.87184327028605, 27.47303545475006;53.872089969925796, 27.471136450767517;53.872520109406906, 27.469763159751892;53.87301982471952, 27.468164563179016;53.873816826659684, 27.466447949409485;53.873121032156966, 27.465235590934753;53.87253908604683, 27.46434509754181;53.87212792358738, 27.463669180870056;53.87133088946145, 27.462381720542908;53.87040732298545, 27.460986971855164;53.869534344143496, 27.45978534221649;53.86857910728852, 27.458530068397522;53.86761119603944, 27.45733916759491;53.86668122099755, 27.45611608028412;53.86580816438814, 27.45510756969452;53.86517550328544, 27.45410978794098;53.86458711987318, 27.45330512523651;53.86391647922593, 27.451792359352112;53.86339767436902, 27.44960367679596;53.86296111407811, 27.446889281272888;53.86239168076566, 27.44330585002899;53.86195510997748, 27.44181454181671;53.86118319105908, 27.439776062965393;53.859993648777994, 27.437780499458313;53.85896226293885, 27.436535954475403;53.85807006204299, 27.435302138328552;53.85707026822417, 27.433907389640808;53.85624763514402, 27.432298064231873;53.856222323100496, 27.43186891078949;53.855842640610355, 27.43182599544525;53.85522248513489, 27.431032061576843;53.85496302960726, 27.430441975593567;53.85458333569178, 27.429926991462708;53.853558144912896, 27.42683708667755;53.852817713732044, 27.42485225200653;53.852539257590884, 27.424433827400208;53.852210170671896, 27.4236398935318;53.850944427638, 27.42164433002472;53.85020395021714, 27.420571446418762;53.849507762916666, 27.41959512233734;53.84869763948151, 27.418468594551086;53.84801408563753, 27.41736352443695;53.84684948671356, 27.41536796092987;53.84560257042958, 27.41309344768524;53.843950507392286, 27.409284710884094;53.84333650566219, 27.407568097114563;53.84248195218477, 27.405475974082947;53.84241865123334, 27.40365207195282;53.84253892295923, 27.40261137485504;53.842279388803306, 27.399993538856506;53.84300734638799, 27.399789690971375;53.84343778491824, 27.397279143333435;53.843095966447144, 27.395777106285095;53.84282377566876, 27.395755648612976;53.84260222372883, 27.396485209465027;");
        paths.Add("53.873854778754165, 27.46636211872101;53.87405718934307, 27.466769814491272;53.87355748642556, 27.46777832508087;53.87317796123293, 27.468491792678833;53.87264662017708, 27.47033715248108;53.87270987541868, 27.471131086349487;53.87445568232711, 27.470701932907104;53.87459483771271, 27.472461462020874;53.874911098230754, 27.472246885299683;53.87506290242987, 27.471431493759155;53.874949049332194, 27.473148107528687;53.87497435004735, 27.47430682182312;53.8757713147396, 27.474671602249146;53.876998357665705, 27.475701570510864;53.87726400139301, 27.476259469985962;53.87740314743382, 27.480207681655884;53.87898431991658, 27.482482194900513;53.88112829429636, 27.485491633415222;53.88353776261037, 27.48900532722473;53.88444839164038, 27.490421533584595;53.88458751377324, 27.49331831932068;53.884536923960326, 27.49698758125305;53.88476457763635, 27.497974634170532;53.88590282742065, 27.498446702957153;53.8864972344337, 27.501858472824097;53.88898859314576, 27.500656843185425;53.89103721341464, 27.499712705612183;53.89270638531865, 27.49902606010437;53.894628379476444, 27.497652769088745;53.89733419510563, 27.496923208236694;53.900558185891306, 27.49655842781067;53.902909645797564, 27.496215105056763;53.90632281983875, 27.49576449394226;53.90900259808204, 27.495356798171997;53.91012754808751, 27.496408224105835;53.9106836919537, 27.495485544204712;53.91309777607136, 27.49529242515564;53.91572656758774, 27.495163679122925;53.91790025095571, 27.496236562728882;53.91989690689219, 27.498704195022583;53.92257581421541, 27.501944303512573;53.92492603473289, 27.5047767162323;53.926922354675604, 27.506879568099976;53.92807209055305, 27.508832216262817;53.92879223865232, 27.511578798294067;53.92901965126006, 27.515591382980347;53.92914599106227, 27.518681287765503;53.92923442869628, 27.522200345993042;53.929550275859334, 27.530332803726196;53.92923442869628, 27.53046154975891;53.92884277489447, 27.532671689987183;53.92818579897376, 27.533572912216187;53.92805945626496, 27.534624338150024;53.92598738127275, 27.535825967788696;53.924382715942045, 27.535675764083862;53.92301807011841, 27.534431219100952;53.92208300926159, 27.533830404281616;53.922689537609386, 27.535182237625122;53.92241154654386, 27.536083459854126;53.922740081240605, 27.536898851394653;53.92375094101389, 27.537800073623657;53.92457224655598, 27.538615465164185;53.92435744512851, 27.540847063064575;53.92342241427249, 27.543164491653442;53.92181765033843, 27.544924020767212;53.91990954365099, 27.54721999168396;53.91897441316794, 27.544602155685425;53.9193282487869, 27.544108629226685;53.92037710103825, 27.54245638847351;53.92016227802405, 27.541619539260864;53.92051610357607, 27.540417909622192;53.92097101773676, 27.539860010147095;53.921261654744285, 27.54069685935974;53.92093310841267, 27.54168391227722;53.92014964134174, 27.541126012802124;53.915360063313855, 27.54099726676941;53.914614406757885, 27.539323568344116;53.91324944161867, 27.535804510116577;53.91196026690914, 27.52994656562805;53.91121454964488, 27.526642084121704;53.90934387825173, 27.528573274612427;53.9089014774962, 27.528444528579712;53.90809250399613, 27.528337240219116;53.90795346009784, 27.52771496772766;53.90777649446699, 27.52822995185852;53.907220311888786, 27.528058290481567;53.906487150811195, 27.528380155563354;53.90575397686496, 27.533615827560425;53.90186035477259, 27.533894777297974;53.900128334826405, 27.534645795822144;53.89690431087159, 27.537628412246704;53.89574107253449, 27.536813020706177;53.89386970813166, 27.53074049949646;53.89255464517303, 27.527393102645874;53.88910240801722, 27.521148920059204;53.886611056088995, 27.514454126358032;53.88513135039567, 27.510355710983276;53.883158327993506, 27.505141496658325;53.88145082960089, 27.50106453895569;53.87879458237561, 27.498167753219604;53.87665681325095, 27.496837377548218;53.87410146652881, 27.49501347541809;53.872608666986224, 27.493854761123657;53.872305040219324, 27.490314245224;53.87238094711771, 27.48707413673401;53.871723082754876, 27.483233213424683;53.87168512872593, 27.47733235359192;53.87173573409021, 27.473362684249878;53.872431551640084, 27.470144033432007;53.87378519988795, 27.466517686843872;");
        paths.Add("53.84264020414466, 27.396538853645325;53.842893406036325, 27.395605444908142;53.84312761642283, 27.39592730998993;53.84333650566219, 27.397450804710388;53.84302633641647, 27.399843335151672;53.84296936630521, 27.403287291526794;53.84248828227466, 27.40367352962494;53.8425452530405, 27.40543305873871;53.842982026336635, 27.406484484672546;53.84348842445437, 27.407761216163635;53.844266999612415, 27.409939169883728;53.84483667957947, 27.41135537624359;53.84548863714065, 27.412685751914978;53.84621653892655, 27.41414487361908;53.84687480442599, 27.415314316749573;53.84750774226031, 27.416301369667053;53.84803940264559, 27.417256236076355;53.84874827265557, 27.41835057735443;53.849533079021384, 27.419444918632507;53.85024192373689, 27.420732378959656;53.85114062031497, 27.422030568122864;53.852153213057846, 27.423672080039978;53.85256457186207, 27.42436945438385;53.852849356358156, 27.424755692481995;53.85356447332807, 27.426794171333313;53.85386823613101, 27.427706122398376;53.85469091598426, 27.429991364479065;53.85499467061136, 27.43038833141327;53.85522248513489, 27.43113934993744;53.855924905442215, 27.431997656822205;53.856405835069175, 27.431697249412537;53.85655137847215, 27.432233691215515;53.85717151425485, 27.433478236198425;53.857981473681804, 27.434797883033752;53.85926598656308, 27.436439394950867;53.86030369231191, 27.43788778781891;53.8611958455824, 27.439507842063904;53.86187285700016, 27.44087040424347;53.86254985746321, 27.44281232357025;53.86272068762485, 27.44334876537323;53.862935806095464, 27.44482934474945;53.86319521219184, 27.446513772010803;53.86353686654749, 27.448981404304504;53.86373932707131, 27.450408339500427;53.8641442451796, 27.45159924030304;53.86479590203119, 27.452961802482605;53.86564367342188, 27.454496026039124;53.86676979093991, 27.455965876579285;53.868073012697124, 27.45757520198822;53.869730449673014, 27.45961368083954;53.87090706354214, 27.461147904396057;53.8718748985263, 27.462456822395325;53.8730071737726, 27.464688420295715;53.87396863483098, 27.466211915016174;53.87508187791601, 27.46429145336151;53.87534120869701, 27.464731335639954;53.87453158532212, 27.465911507606506;53.87384845340747, 27.46731698513031;53.87311470669931, 27.468647360801697;53.87270987541868, 27.469902634620667;53.87215955161238, 27.472262978553772;53.872007736874224, 27.47426927089691;53.87199508562118, 27.476007342338562;53.87202038812345, 27.47831404209137;53.87201406249933, 27.480416893959045;53.87206466746561, 27.483131289482117;53.872083644312184, 27.48432219028473;53.87238727268634, 27.48680055141449;53.872431551640084, 27.48877465724945;53.87228606347322, 27.49030888080597;53.87253908604683, 27.491918206214905;53.87278578158229, 27.494085431098938;53.874550561049354, 27.49527633190155;53.876163466393564, 27.496177554130554;53.876827585807035, 27.4965637922287;53.87691613426526, 27.497454285621643;53.878294936065494, 27.49851644039154;53.87943968650217, 27.49931037425995;53.88049586490811, 27.5004905462265;53.88128640014869, 27.502110600471497;53.882291939376145, 27.504363656044006;53.88228561530564, 27.50523269176483;53.88115991551464, 27.507324814796448;53.88011640268335, 27.509706616401672;53.87923097743271, 27.512335181236267;53.87842142936666, 27.51448094844818;53.877845881755896, 27.51621901988983;53.8769098093816, 27.518890500068665;53.875999016147695, 27.521143555641174;53.87516410492313, 27.523611187934875;53.87411411714471, 27.526615262031555;53.873121032156966, 27.52897560596466;53.87253908604683, 27.530938982963562;53.87255173713532, 27.534136176109314;53.872577039300836, 27.534930109977722;53.872937593496225, 27.534930109977722;53.872988197345066, 27.535369992256165;53.872988197345066, 27.53796637058258;53.873032475662576, 27.540670037269592;53.872874338599054, 27.542869448661804;53.87257071376089, 27.546345591545105;53.87234931926012, 27.549124360084534;53.87212792358738, 27.551280856132507;53.87202671374662, 27.554638981819153;53.872083644312184, 27.556377053260803;53.87207731869761, 27.559896111488342;53.87215955161238, 27.562342286109924;53.87434182757628, 27.562760710716248;53.87451260958627, 27.564069628715515;53.87481622032648, 27.56391942501068;53.87506290242987, 27.564509510993958;53.875132479170304, 27.564885020256042;53.874835195924554, 27.56498157978058;53.874752968270705, 27.564563155174255;53.87490477304383, 27.564380764961243;53.87519573065205, 27.56472408771515;53.875480361136056, 27.565013766288757;53.8757523395663, 27.565861344337463;53.876125516394225, 27.567352652549744;53.875904140711654, 27.56791055202484;53.87471501699131, 27.568361163139343;53.874803569922975, 27.56923019886017;53.87413941836498, 27.570088505744934;53.87333609714841, 27.57120430469513;53.87280475810167, 27.571805119514465;53.87289331507824, 27.573242783546448;53.872507458308846, 27.57342517375946;53.872273412304345, 27.572041153907776;53.87110316263567, 27.571300864219666;53.86999613956889, 27.570485472679138;53.868110970003876, 27.57064640522003;53.86623836847283, 27.570989727973938;53.86430874204168, 27.5713974237442;53.86236004535441, 27.571805119514465;53.860689661664345, 27.57216989994049;53.86089846327257, 27.574573159217834;53.86112624565677, 27.5773948431015;53.86122115461755, 27.578735947608948;53.86091111788199, 27.579755187034607;53.86048718638244, 27.581536173820496;53.86065802496613, 27.582287192344666;53.8611135911124, 27.584003806114197;53.86070864367176, 27.585076689720154;53.86105031833316, 27.586535811424255;53.86115788200094, 27.58790910243988;53.859455812708674, 27.58969008922577;53.858462379673895, 27.5903981924057;53.85867119239692, 27.591084837913513;53.8589242973308, 27.591353058815002;53.858791417431405, 27.59162127971649;53.859171073167325, 27.593252062797546;53.85973422283119, 27.595033049583435;53.8604808590141, 27.597822546958923;53.8611135911124, 27.599721550941467;53.861613442704765, 27.601266503334045;53.86220819504732, 27.603315711021423;53.86261312797477, 27.604785561561584;53.862872536071876, 27.605697512626648;53.86343563591817, 27.608315348625183;53.864055669678315, 27.61011779308319;53.86476426843789, 27.611952424049377;53.8653020362714, 27.61367976665497;53.86597265470762, 27.615686058998108;53.86682040225137, 27.618175148963928;53.867453038477784, 27.619816660881042;53.868326060758214, 27.622219920158386;53.870084702492484, 27.623206973075867;53.871868572880146, 27.624226212501526;53.87365236718557, 27.625245451927185;53.87560686295154, 27.6263290643692;53.8772387020631, 27.627047896385193;53.87933216983891, 27.625696063041687;53.88138126337311, 27.62360394001007;53.882696677899176, 27.622230648994446;53.88517561585588, 27.619709372520447;53.88683869846981, 27.617939114570618;53.888236142587296, 27.616533637046814;53.89023421681812, 27.614055275917053;53.8918781303201, 27.612220644950867;53.89345243302114, 27.610310912132263;53.89527956139604, 27.60841190814972;53.89735948227577, 27.605965733528137;53.89811176858813, 27.605085968971252;53.89856060523087, 27.606555819511414;53.899540443121516, 27.609044909477234;53.90114606327577, 27.613186240196228;53.90297285525866, 27.617944478988647;53.9034943296634, 27.619132697582245;53.90400947677477, 27.620699107646942;53.90483433100574, 27.619915902614594;53.90535894246443, 27.619422376155853;53.90608264263912, 27.619180977344513;53.90649347122031, 27.61896640062332;53.906970659346094, 27.618864476680756;53.906980139849644, 27.617056667804718;53.90697697968204, 27.61603206396103;53.90715078854579, 27.615914046764374;53.907542645876184, 27.615565359592438;53.90776069392778, 27.615125477313995;53.90799138120691, 27.614433467388153;53.90812410481754, 27.61341154575348;53.90847171227521, 27.611984610557556;53.90952083724188, 27.610825896263123;53.91091120321902, 27.609087824821472;53.912396315750094, 27.607274651527405;53.913773947390126, 27.605461478233337;53.915069385223056, 27.60393798351288;53.91690820601384, 27.603841423988342;53.918386786675605, 27.603766322135925;53.91968208140718, 27.602832913398743;53.92227255031691, 27.601073384284973;53.92327078567368, 27.59991466999054;53.92454697585717, 27.598584294319153;53.9259747463536, 27.594056725502014;53.92721926752343, 27.589990496635437;53.92827423864236, 27.58646070957184;53.92944920502722, 27.58267343044281;53.930680989107415, 27.578371167182922;53.93039041767784, 27.577266097068787;53.93016301253918, 27.577105164527893;53.93013774522507, 27.57621467113495;53.92809104197802, 27.573886513710022;53.926088460488145, 27.57192313671112;53.92404155866841, 27.570067048072815;53.92133115459852, 27.56794273853302;53.91879749423395, 27.565636038780212;53.91677551031722, 27.564101815223694;53.91480398172081, 27.5624817609787;53.912939790540534, 27.561001181602478;53.910456179448985, 27.55899488925934;53.908857237162934, 27.55766451358795;53.90693589748128, 27.55618393421173;53.905785579455625, 27.554767727851868;53.905166164320605, 27.55370557308197;53.904660512503085, 27.552761435508728;53.90498286624398, 27.5525039434433;53.90461626767786, 27.552160620689392;53.9038514568639, 27.55119502544403;53.90191724472071, 27.548405528068542;53.900741503382605, 27.54693567752838;53.90026740477252, 27.54625976085663;53.899591014776156, 27.544779181480408;53.89904736628095, 27.54323422908783;53.897738787991734, 27.540412545204163;53.89696120756826, 27.538556456565857;53.89706867889506, 27.53769814968109;53.896986494964146, 27.537140250205994;53.89574107253449, 27.53651797771454;53.894799078630896, 27.53369629383087;53.89365474874657, 27.529587149620056;53.89283915749413, 27.527838349342346;53.89170741923439, 27.525617480278015;53.89046816230106, 27.523428797721863;53.88927312974328, 27.521079182624817;53.88777454856646, 27.517367005348206;53.88707266295829, 27.515199780464172;53.887205453031335, 27.5145024061203;53.88712957304123, 27.513901591300964;53.88658576241459, 27.513858675956726;53.885415913250675, 27.51080095767975;53.8850934085353, 27.509331107139587;53.88391719378349, 27.506069540977478;53.88400572722838, 27.505179047584534;53.88379071710842, 27.50503957271576;53.88339863698405, 27.50403106212616;53.88303817298023, 27.503677010536194;53.88259549363577, 27.502593398094177;53.88205794813009, 27.501445412635803;53.88164687925638, 27.500898241996765;53.881052403274865, 27.50024378299713;53.880059483052605, 27.49904215335846;53.87944601100317, 27.498355507850647;53.87883885454552, 27.497937083244324;53.877940752786486, 27.49729335308075;53.87713750459046, 27.49679982662201;53.876523989669614, 27.496477961540222;53.87511982886247, 27.49552309513092;53.87429755064501, 27.494997382164;53.87306410300353, 27.494085431098938;53.87270987541868, 27.493924498558044;53.87251378385836, 27.492433190345764;53.872425226078136, 27.491349577903748;53.8722354587748, 27.490169405937195;53.87245052832018, 27.488495707511902;53.87234299368575, 27.486714720726013;53.87200141124817, 27.484354376792908;53.87195713183904, 27.48228371143341;53.87195713183904, 27.47982680797577;53.87193815493508, 27.478442788124084;53.87191917802248, 27.475857138633728;53.87184327028605, 27.47303545475006;53.872089969925796, 27.471136450767517;53.872520109406906, 27.469763159751892;53.87301982471952, 27.468164563179016;53.873816826659684, 27.466447949409485;53.873121032156966, 27.465235590934753;53.87253908604683, 27.46434509754181;53.87212792358738, 27.463669180870056;53.87133088946145, 27.462381720542908;53.87040732298545, 27.460986971855164;53.869534344143496, 27.45978534221649;53.86857910728852, 27.458530068397522;53.86761119603944, 27.45733916759491;53.86668122099755, 27.45611608028412;53.86580816438814, 27.45510756969452;53.86517550328544, 27.45410978794098;53.86458711987318, 27.45330512523651;53.86391647922593, 27.451792359352112;53.86339767436902, 27.44960367679596;53.86296111407811, 27.446889281272888;53.86239168076566, 27.44330585002899;53.86195510997748, 27.44181454181671;53.86118319105908, 27.439776062965393;53.859993648777994, 27.437780499458313;53.85896226293885, 27.436535954475403;53.85807006204299, 27.435302138328552;53.85707026822417, 27.433907389640808;53.85624763514402, 27.432298064231873;53.856222323100496, 27.43186891078949;53.855842640610355, 27.43182599544525;53.85522248513489, 27.431032061576843;53.85496302960726, 27.430441975593567;53.85458333569178, 27.429926991462708;53.853558144912896, 27.42683708667755;53.852817713732044, 27.42485225200653;53.852539257590884, 27.424433827400208;53.852210170671896, 27.4236398935318;53.850944427638, 27.42164433002472;53.85020395021714, 27.420571446418762;53.849507762916666, 27.41959512233734;53.84869763948151, 27.418468594551086;53.84801408563753, 27.41736352443695;53.84684948671356, 27.41536796092987;53.84560257042958, 27.41309344768524;53.843950507392286, 27.409284710884094;53.84333650566219, 27.407568097114563;53.84248195218477, 27.405475974082947;53.84241865123334, 27.40365207195282;53.84253892295923, 27.40261137485504;53.842279388803306, 27.399993538856506;53.84300734638799, 27.399789690971375;53.84343778491824, 27.397279143333435;53.843095966447144, 27.395777106285095;53.84282377566876, 27.395755648612976;53.84260222372883, 27.396485209465027;");
        paths.Add("53.873861104099895, 27.466431856155396;53.874645439556346, 27.46511220932007;53.876467065148454, 27.467944622039795;53.87656826424359, 27.467472553253174;53.8773778481881, 27.46953248977661;53.877605540848414, 27.471249103546143;53.87768143812634, 27.472236156463623;53.87740314743382, 27.47455358505249;53.877301950359126, 27.476141452789307;53.87735254892709, 27.48030424118042;53.88094489075861, 27.485625743865967;53.87930054723829, 27.488973140716553;53.87775733526651, 27.491891384124756;53.876720062427104, 27.494595050811768;53.87649236494519, 27.496869564056396;53.87993299470686, 27.49948740005493;53.88263976178112, 27.504379749298096;53.88218443004732, 27.50549554824829;53.88279153792373, 27.50678300857544;53.88377806941986, 27.507212162017822;53.88582694506591, 27.5127911567688;53.888811547174164, 27.52124547958374;53.89318689214511, 27.52922773361206;53.89571578438492, 27.537424564361572;53.895564055166155, 27.538626194000244;53.89349036729426, 27.54051446914673;53.89331334039174, 27.541544437408447;53.891593611456514, 27.540643215179443;53.89121424995872, 27.541587352752686;53.888634500452646, 27.545106410980225;53.887496325050655, 27.54420518875122;53.88691457898791, 27.544634342193604;53.887496325050655, 27.54622220993042;53.88592812150827, 27.548410892486572;53.884739282844684, 27.548539638519287;53.885017524711294, 27.54948377609253;53.88567517994322, 27.550041675567627;53.88532105918089, 27.553603649139404;53.88494164074951, 27.557895183563232;53.88491634606499, 27.561585903167725;53.882943313515305, 27.562744617462158;53.88061602722799, 27.56270170211792;53.87707425604571, 27.56192922592163;53.87431652647844, 27.561542987823486;53.87315265943097, 27.56141424179077;53.87239359825404, 27.55922555923462;53.87181164202185, 27.559311389923096;53.8722923890562, 27.562272548675537;53.87439242972601, 27.562830448150635;53.874468332835825, 27.564246654510498;53.87484784631849, 27.56471872329712;53.875043926935106, 27.56641387939453;53.87538548452331, 27.566376328468323;53.87550566152977, 27.564895749092102;53.87516410492314, 27.564874291419983;53.87494272415101, 27.564992308616638;53.87535385893788, 27.56593644618988;53.875619513112056, 27.56621539592743;53.87565746357061, 27.5669664144516;53.87574601450659, 27.56818950176239;53.875126154016854, 27.56844699382782;53.87520205579496, 27.569122910499573;53.87467706567749, 27.569337487220764;53.87351953406129, 27.571102380752563;53.87273517748852, 27.571810483932495;53.873165310333874, 27.576273679733276;53.87339302593161, 27.579041719436646;53.873532184853204, 27.580243349075317;53.87460748817937, 27.58045792579651;53.87525265690383, 27.581058740615845;53.875720714258314, 27.581230401992798;53.8757713147396, 27.58191704750061;53.87621406633912, 27.58266806602478;53.87674536207079, 27.580736875534058;53.877011007405294, 27.57938504219055;53.87669476276812, 27.578290700912476;53.876125516394225, 27.577582597732544;53.87585986543452, 27.57567286491394;53.876340565935116, 27.574020624160767;53.8771881033574, 27.57339835166931;53.87823801395514, 27.573548555374146;53.879123460232776, 27.57468581199646;53.88037570224287, 27.575587034225464;53.88136229074544, 27.575587034225464;53.882209726384865, 27.57470726966858;53.882829481873266, 27.573548555374146;53.883360693551055, 27.573763132095337;53.8840057272284, 27.572389841079712;53.884119555667695, 27.571574449539185;53.88489105136512, 27.570995092391968;53.88571312127557, 27.570759057998657;53.886345471744505, 27.57120966911316;53.88716751305349, 27.571295499801636;53.88796424535638, 27.57168173789978;53.88926680672882, 27.568892240524292;53.89010143636691, 27.5662100315094;53.89063255564083, 27.566381692886353;53.891328058769616, 27.56466507911682;53.89206148589475, 27.563527822494507;53.89287076985469, 27.563291788101196;53.89355359100628, 27.564150094985962;53.894261690046235, 27.565330266952515;53.89467895707639, 27.566789388656616;53.89480540080841, 27.568141222000122;53.89467895707639, 27.56990075111389;53.89370532752489, 27.571810483932495;53.892605225282786, 27.573870420455933;53.89300986395706, 27.574385404586792;53.89239025939351, 27.575372457504272;53.89154303012242, 27.576981782913208;53.89096134038051, 27.57841944694519;53.89079694833395, 27.58045792579651;53.89206148589475, 27.582796812057495;53.89283283501914, 27.58238911628723;53.893806484897894, 27.581273317337036;53.8940214435025, 27.581745386123657;53.89461573506691, 27.58118748664856;53.89553876690946, 27.580114603042603;53.89647442221495, 27.578548192977905;53.89730890792014, 27.577368021011353;53.89832038340205, 27.575576305389404;53.899761693658625, 27.572872638702393;53.89950883580628, 27.572057247161865;53.90231547218853, 27.56892442703247;53.90307399019748, 27.567894458770752;53.90416117532688, 27.566649913787842;53.904818529353584, 27.56566286087036;53.90631017896792, 27.563860416412354;53.90588038708414, 27.562358379364014;53.905703412670185, 27.560856342315674;53.90658827724216, 27.559010982513428;53.90641130582731, 27.556564807891846;53.90653771405727, 27.555148601531982;53.90625961544641, 27.554333209991455;53.90565284841419, 27.55373239517212;53.90509663755863, 27.552831172943115;53.902037345467924, 27.548325061798096;53.90041911691308, 27.546350955963135;53.89918011831045, 27.54399061203003;53.897005460501035, 27.53901243209839;53.89725833350332, 27.53720998764038;53.89596866519194, 27.53669500350952;");
        paths.Add("53.842725, 27.394742;53.842421, 27.394323;53.842295, 27.393519;53.840712, 27.392789;53.842327, 27.405739;53.845466, 27.412927;53.844947, 27.416532;53.845935, 27.421757;53.845922, 27.424042;53.846700, 27.431080;53.847770, 27.435608;53.849080, 27.438076;53.849928, 27.439621;53.850992, 27.437791; 53.850960, 27.435506; 53.854800, 27.431003; 53.859336, 27.427977; 53.901734, 27.407292; 53.922108, 27.415103; 53.943887, 27.436217;53.962373, 27.460764; 53.966766, 27.475527; 53.971562, 27.584103; 53.959999, 27.627362; 53.962676, 27.639550; 53.977771, 27.652253; 53.999797, 27.657067;54.015576, 27.660675; 54.032874, 27.684204; 54.040589, 27.697051; 54.059063, 27.707011; 54.074311, 27.732994; 54.100219, 27.790878; 54.114098, 27.805169;54.156722, 27.811376; 54.168479, 27.818218; 54.177143, 27.818218; 54.177979, 27.813421; 54.181085, 27.806684");


        var pathTextures = new Texture2D[paths.Count];
        for (int i = 0; i < paths.Count; i++) pathTextures[i] = null;

        for (int i = 0; i < paths.Count; i++)
        {
            StartCoroutine(GetImage(threadId++, GetPathFromString(paths[i]), i, (x) => { pathTextures[x.Index] = x.Texture; }));
            if (i % 9 == 0) yield return new WaitForSeconds(1f); //не более 9 потоков за сек
        }
        while (baseTexture == null || HaveEmpty(pathTextures)) yield return null;

        Debug.Log("all data recieved");


        Texture2D outTexture = baseTexture;

        List<Point> points = new List<Point>();
        for (int i = 0; i < paths.Count; i++)
        {
            for (int w = 0; w <= size; w++)
            {
                for (int h = 0; h <= size; h++)
                {
                    if (baseTexture.GetPixel(w, h) != pathTextures[i].GetPixel(w, h))
                    {
                        //if (points.Count < 10000)
                            if (!points.Contains(new Point(w, h))) points.Add(new Point(w, h));
                    }
                }
            }
        }
        Debug.Log("points=" + points.Count);
        foreach(var p in points)
        {
            outTexture.SetPixel(p.x, p.y, Color.red);
        }
        outTexture.Apply();

        Sprite spr = Sprite.Create(outTexture, new Rect(0, 0, size, size), new Vector2(size, size));
        GameObject.Find("Map").GetComponent<Image>().sprite = spr;

        Debug.Log("process END");
    }
    public class ActionResults
    {
        public Texture2D Texture;
        public int Index;
        public ActionResults(Texture2D texture, int index)
        {
            Texture = texture;
            Index = index;
        }
    }
    public IEnumerator GetImage(int threadId, GoogleMapPath path, int index, Action<ActionResults> action)
    {
        Debug.Log("act=" + threadId + " BEGIN");

        var image = new Texture2D(size, size);
        var qs = "";
        if (!autoLocateCenter)
        {
            if (!string.IsNullOrEmpty(centerLocation.address)) qs += "center=" + HTTP.URL.Encode(centerLocation.address);
            else qs += "center=" + HTTP.URL.Encode(string.Format("{0},{1}", centerLocation.latitude, centerLocation.longitude));
            qs += "&zoom=" + zoom.ToString();
        }
        qs += "&size=" + HTTP.URL.Encode(string.Format("{0}x{0}", size));
        qs += "&scale=" + (doubleResolution ? "2" : "1");
        qs += "&maptype=" + mapType.ToString().ToLower();
        qs += "&format=png32";//только его не размывает
        
        //var usingSensor = false;
        //qs += "&sensor=" + (usingSensor ? "true" : "false");

        foreach (var i in markers)
        {
            qs += "&markers=" + string.Format("size:{0}|color:{1}|label:{2}", i.size.ToString().ToLower(), i.color, i.label);
            foreach (var loc in i.locations)
            {
                if (loc.address != "")
                    qs += "|" + HTTP.URL.Encode(loc.address);
                else
                    qs += "|" + HTTP.URL.Encode(string.Format("{0},{1}", loc.latitude, loc.longitude));
            }
        }

        if (path != null)
        {
            qs += "&path=" + string.Format("weight:{0}|color:{1}", path.weight, "0xff0000ff");
            if (path.fill) qs += "|fillcolor:" + path.fillColor;
            qs += "|enc:" + EncodePolyline.EncodeCoordinates(path.locations);
        }
        qs += "&key=AIzaSyAzduON1ycPY7318RfjwIjI3vtnWN8xb_s";

        //Debug.Log("before get =" + url + "?" + qs);
        //Debug.Log("url len=" + (url + "?" + qs).Length);
        var req = new HTTP.Request("GET", url + "?" + qs, true);
        req.Send();

        while (!req.isDone) yield return null;

        //Debug.Log("after get=" + req.response.Bytes.Length);
        try
        {
            //Debug.Log("act=" + threadId + " after get data=" + req.response.message);
        }
        catch
        {

        }

        if (req.exception == null) image.LoadImage(req.response.Bytes);
        else
        {
            //Debug.Log("after get ex=" + req.exception.Message);
        }
        Debug.Log("act=" + threadId+" END");
        action(new ActionResults(image, index));
    }
}























public enum GoogleMapColor
{
    black,
    brown,
    green,
    purple,
    yellow,
    blue,
    gray,
    orange,
    red,
    white
}

public class Point
{
    public int x;
    public int y;
    public Point (int _x,int _y)
    {
        x = _x;
        y = _y;
    }
}

[System.Serializable]
public class GoogleMapLocation
{
    public string address;
    public float latitude;
    public float longitude;
}

[System.Serializable]
public class GoogleMapMarker
{
    public enum GoogleMapMarkerSize
    {
        Tiny,
        Small,
        Mid
    }
    public GoogleMapMarkerSize size;
    public GoogleMapColor color;
    public string label;
    public List<GoogleMapLocation> locations;
}

[System.Serializable]
public class GoogleMapPath
{
    public int weight = 3;
    public GoogleMapColor color;
    public bool fill = false;
    public GoogleMapColor fillColor;
    public List<GoogleMapLocation> locations;
}
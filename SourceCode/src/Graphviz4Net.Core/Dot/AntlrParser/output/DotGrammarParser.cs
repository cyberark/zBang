// $ANTLR 3.3 Nov 30, 2010 12:45:30 D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g 2013-05-05 00:38:29

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


	using Graphviz4Net.Dot;


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

using Antlr.Runtime.Debug;
using IOException = System.IO.IOException;
namespace Graphviz4Net.Dot.AntlrParser
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3 Nov 30, 2010 12:45:30")]
[System.CLSCompliant(false)]
public partial class DotGrammarParser : DebugAntlr.Runtime.Parser
{
	internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "L_CURL_BRACKET", "R_CURL_BRACKET", "L_SQUARE_BRACKET", "R_SQUARE_BRACKET", "SEMICOLON", "EQUALS", "COMMA", "GRAPH", "ID", "EDGE_OPERATOR_NOARROW", "EDGE_OPERATOR_ARROW", "NODE", "EDGE", "SUBGRAPH", "QUOTED_VALUE", "S", "U", "B", "G", "R", "A", "P", "H", "D", "I", "N", "O", "E", "ALLOWED_QUOTED_VALUES", "STR", "NUMBER", "WS", "C", "F", "J", "K", "L", "M", "Q", "T", "V", "W", "X", "Y", "Z"
	};
	public const int EOF=-1;
	public const int L_CURL_BRACKET=4;
	public const int R_CURL_BRACKET=5;
	public const int L_SQUARE_BRACKET=6;
	public const int R_SQUARE_BRACKET=7;
	public const int SEMICOLON=8;
	public const int EQUALS=9;
	public const int COMMA=10;
	public const int GRAPH=11;
	public const int ID=12;
	public const int EDGE_OPERATOR_NOARROW=13;
	public const int EDGE_OPERATOR_ARROW=14;
	public const int NODE=15;
	public const int EDGE=16;
	public const int SUBGRAPH=17;
	public const int QUOTED_VALUE=18;
	public const int S=19;
	public const int U=20;
	public const int B=21;
	public const int G=22;
	public const int R=23;
	public const int A=24;
	public const int P=25;
	public const int H=26;
	public const int D=27;
	public const int I=28;
	public const int N=29;
	public const int O=30;
	public const int E=31;
	public const int ALLOWED_QUOTED_VALUES=32;
	public const int STR=33;
	public const int NUMBER=34;
	public const int WS=35;
	public const int C=36;
	public const int F=37;
	public const int J=38;
	public const int K=39;
	public const int L=40;
	public const int M=41;
	public const int Q=42;
	public const int T=43;
	public const int V=44;
	public const int W=45;
	public const int X=46;
	public const int Y=47;
	public const int Z=48;

	// delegates
	// delegators

	public static readonly string[] ruleNames =
		new string[]
		{
			"invalidRule", "stmt_list", "opt_id", "dot", "attr_value", "attr_list", 
		"subgraph_stmt", "id", "attr", "opt_attr_list", "stmt"
		};

		int ruleLevel = 0;
		public virtual int RuleLevel { get { return ruleLevel; } }
		public virtual void IncRuleLevel() { ruleLevel++; }
		public virtual void DecRuleLevel() { ruleLevel--; }
		public DotGrammarParser( ITokenStream input )
			: this( input, DebugEventSocketProxy.DefaultDebuggerPort, new RecognizerSharedState() )
		{
		}
		public DotGrammarParser( ITokenStream input, int port, RecognizerSharedState state )
			: base( input, state )
		{
			DebugEventSocketProxy proxy = new DebugEventSocketProxy( this, port, null );
			DebugListener = proxy;
			try
			{
				proxy.Handshake();
			}
			catch ( IOException ioe )
			{
				ReportError( ioe );
			}
		}
	public DotGrammarParser( ITokenStream input, IDebugEventListener dbg )
		: base( input, dbg, new RecognizerSharedState() )
	{

	}
	protected virtual bool EvalPredicate( bool result, string predicate )
	{
		dbg.SemanticPredicate( result, predicate );
		return result;
	}


	public override string[] TokenNames { get { return DotGrammarParser.tokenNames; } }
	public override string GrammarFileName { get { return "D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g"; } }


	partial void OnCreated();
	partial void EnterRule(string ruleName, int ruleIndex);
	partial void LeaveRule(string ruleName, int ruleIndex);

	#region Rules

	partial void Enter_dot();
	partial void Leave_dot();

	// $ANTLR start "dot"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:29:8: public dot : GRAPH opt_id L_CURL_BRACKET stmt_list R_CURL_BRACKET ;
	[GrammarRule("dot")]
	public void dot()
	{
		Enter_dot();
		EnterRule("dot", 1);
		TraceIn("dot", 1);
		try { DebugEnterRule(GrammarFileName, "dot");
		DebugLocation(29, 64);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:29:11: ( GRAPH opt_id L_CURL_BRACKET stmt_list R_CURL_BRACKET )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:29:13: GRAPH opt_id L_CURL_BRACKET stmt_list R_CURL_BRACKET
			{
			DebugLocation(29, 13);
			Match(input,GRAPH,Follow._GRAPH_in_dot116); 
			DebugLocation(29, 19);
			PushFollow(Follow._opt_id_in_dot118);
			opt_id();
			PopFollow();

			DebugLocation(29, 26);
			Match(input,L_CURL_BRACKET,Follow._L_CURL_BRACKET_in_dot120); 
			DebugLocation(29, 41);
			PushFollow(Follow._stmt_list_in_dot122);
			stmt_list();
			PopFollow();

			DebugLocation(29, 51);
			Match(input,R_CURL_BRACKET,Follow._R_CURL_BRACKET_in_dot124); 

			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("dot", 1);
			LeaveRule("dot", 1);
			Leave_dot();
		}
		DebugLocation(29, 64);
		} finally { DebugExitRule(GrammarFileName, "dot"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return;

	}
	// $ANTLR end "dot"


	partial void Enter_id();
	partial void Leave_id();

	// $ANTLR start "id"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:31:1: id returns [string id_content] : token= ID ;
	[GrammarRule("id")]
	private string id()
	{
		Enter_id();
		EnterRule("id", 2);
		TraceIn("id", 2);
		string id_content = default(string);

		IToken token=null;

		try { DebugEnterRule(GrammarFileName, "id");
		DebugLocation(31, 70);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:31:32: (token= ID )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:31:34: token= ID
			{
			DebugLocation(31, 40);
			token=(IToken)Match(input,ID,Follow._ID_in_id140); 
			DebugLocation(31, 45);
			id_content = token.Text;

			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("id", 2);
			LeaveRule("id", 2);
			Leave_id();
		}
		DebugLocation(31, 70);
		} finally { DebugExitRule(GrammarFileName, "id"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return id_content;

	}
	// $ANTLR end "id"


	partial void Enter_opt_id();
	partial void Leave_opt_id();

	// $ANTLR start "opt_id"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:32:1: opt_id : ( id | );
	[GrammarRule("opt_id")]
	private void opt_id()
	{
		Enter_opt_id();
		EnterRule("opt_id", 3);
		TraceIn("opt_id", 3);
		try { DebugEnterRule(GrammarFileName, "opt_id");
		DebugLocation(32, 13);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:32:8: ( id | )
			int alt1=2;
			try { DebugEnterDecision(1, decisionCanBacktrack[1]);
			int LA1_0 = input.LA(1);

			if ((LA1_0==ID))
			{
				alt1=1;
			}
			else if ((LA1_0==L_CURL_BRACKET))
			{
				alt1=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 1, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(1); }
			switch (alt1)
			{
			case 1:
				DebugEnterAlt(1);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:32:10: id
				{
				DebugLocation(32, 10);
				PushFollow(Follow._id_in_opt_id149);
				id();
				PopFollow();


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:32:14: 
				{
				}
				break;

			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("opt_id", 3);
			LeaveRule("opt_id", 3);
			Leave_opt_id();
		}
		DebugLocation(32, 13);
		} finally { DebugExitRule(GrammarFileName, "opt_id"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return;

	}
	// $ANTLR end "opt_id"


	partial void Enter_stmt_list();
	partial void Leave_stmt_list();

	// $ANTLR start "stmt_list"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:34:1: stmt_list : ( ( subgraph_stmt stmt_list ) | ( stmt SEMICOLON stmt_list ) | );
	[GrammarRule("stmt_list")]
	private void stmt_list()
	{
		Enter_stmt_list();
		EnterRule("stmt_list", 4);
		TraceIn("stmt_list", 4);
		try { DebugEnterRule(GrammarFileName, "stmt_list");
		DebugLocation(34, 59);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:35:2: ( ( subgraph_stmt stmt_list ) | ( stmt SEMICOLON stmt_list ) | )
			int alt2=3;
			try { DebugEnterDecision(2, decisionCanBacktrack[2]);
			switch (input.LA(1))
			{
			case SUBGRAPH:
				{
				alt2=1;
				}
				break;
			case SEMICOLON:
			case GRAPH:
			case ID:
			case NODE:
			case EDGE:
				{
				alt2=2;
				}
				break;
			case R_CURL_BRACKET:
				{
				alt2=3;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 2, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(2); }
			switch (alt2)
			{
			case 1:
				DebugEnterAlt(1);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:35:4: ( subgraph_stmt stmt_list )
				{
				DebugLocation(35, 4);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:35:4: ( subgraph_stmt stmt_list )
				DebugEnterAlt(1);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:35:5: subgraph_stmt stmt_list
				{
				DebugLocation(35, 5);
				PushFollow(Follow._subgraph_stmt_in_stmt_list163);
				subgraph_stmt();
				PopFollow();

				DebugLocation(35, 19);
				PushFollow(Follow._stmt_list_in_stmt_list165);
				stmt_list();
				PopFollow();


				}


				}
				break;
			case 2:
				DebugEnterAlt(2);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:35:32: ( stmt SEMICOLON stmt_list )
				{
				DebugLocation(35, 32);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:35:32: ( stmt SEMICOLON stmt_list )
				DebugEnterAlt(1);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:35:33: stmt SEMICOLON stmt_list
				{
				DebugLocation(35, 33);
				PushFollow(Follow._stmt_in_stmt_list171);
				stmt();
				PopFollow();

				DebugLocation(35, 38);
				Match(input,SEMICOLON,Follow._SEMICOLON_in_stmt_list173); 
				DebugLocation(35, 48);
				PushFollow(Follow._stmt_list_in_stmt_list175);
				stmt_list();
				PopFollow();


				}


				}
				break;
			case 3:
				DebugEnterAlt(3);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:35:60: 
				{
				}
				break;

			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("stmt_list", 4);
			LeaveRule("stmt_list", 4);
			Leave_stmt_list();
		}
		DebugLocation(35, 59);
		} finally { DebugExitRule(GrammarFileName, "stmt_list"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return;

	}
	// $ANTLR end "stmt_list"


	partial void Enter_stmt();
	partial void Leave_stmt();

	// $ANTLR start "stmt"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:37:1: stmt : ( GRAPH graph_attrs= opt_attr_list | vertex_id= id stmt_opt_attr_list= opt_attr_list | source_id= id ( EDGE_OPERATOR_NOARROW | EDGE_OPERATOR_ARROW ) dest_id= id edge_attrs= opt_attr_list | NODE opt_attr_list | EDGE opt_attr_list | );
	[GrammarRule("stmt")]
	private void stmt()
	{
		Enter_stmt();
		EnterRule("stmt", 5);
		TraceIn("stmt", 5);
		IDictionary<string, string> graph_attrs = default(IDictionary<string, string>);
		string vertex_id = default(string);
		IDictionary<string, string> stmt_opt_attr_list = default(IDictionary<string, string>);
		string source_id = default(string);
		string dest_id = default(string);
		IDictionary<string, string> edge_attrs = default(IDictionary<string, string>);

		try { DebugEnterRule(GrammarFileName, "stmt");
		DebugLocation(37, 21);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:37:6: ( GRAPH graph_attrs= opt_attr_list | vertex_id= id stmt_opt_attr_list= opt_attr_list | source_id= id ( EDGE_OPERATOR_NOARROW | EDGE_OPERATOR_ARROW ) dest_id= id edge_attrs= opt_attr_list | NODE opt_attr_list | EDGE opt_attr_list | )
			int alt3=6;
			try { DebugEnterDecision(3, decisionCanBacktrack[3]);
			switch (input.LA(1))
			{
			case GRAPH:
				{
				alt3=1;
				}
				break;
			case ID:
				{
				int LA3_2 = input.LA(2);

				if ((LA3_2==L_SQUARE_BRACKET||LA3_2==SEMICOLON))
				{
					alt3=2;
				}
				else if (((LA3_2>=EDGE_OPERATOR_NOARROW && LA3_2<=EDGE_OPERATOR_ARROW)))
				{
					alt3=3;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 3, 2, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
				}
				break;
			case NODE:
				{
				alt3=4;
				}
				break;
			case EDGE:
				{
				alt3=5;
				}
				break;
			case SEMICOLON:
				{
				alt3=6;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 3, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(3); }
			switch (alt3)
			{
			case 1:
				DebugEnterAlt(1);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:38:2: GRAPH graph_attrs= opt_attr_list
				{
				DebugLocation(38, 2);
				Match(input,GRAPH,Follow._GRAPH_in_stmt191); 
				DebugLocation(38, 20);
				PushFollow(Follow._opt_attr_list_in_stmt197);
				graph_attrs=opt_attr_list();
				PopFollow();

				DebugLocation(38, 36);
				 this.AddGraphAttributes(graph_attrs); 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:39:2: vertex_id= id stmt_opt_attr_list= opt_attr_list
				{
				DebugLocation(39, 12);
				PushFollow(Follow._id_in_stmt208);
				vertex_id=id();
				PopFollow();

				DebugLocation(39, 36);
				PushFollow(Follow._opt_attr_list_in_stmt214);
				stmt_opt_attr_list=opt_attr_list();
				PopFollow();

				DebugLocation(39, 52);
				 this.AddVertex(vertex_id, stmt_opt_attr_list); 

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:40:2: source_id= id ( EDGE_OPERATOR_NOARROW | EDGE_OPERATOR_ARROW ) dest_id= id edge_attrs= opt_attr_list
				{
				DebugLocation(40, 12);
				PushFollow(Follow._id_in_stmt225);
				source_id=id();
				PopFollow();

				DebugLocation(40, 17);
				if ((input.LA(1)>=EDGE_OPERATOR_NOARROW && input.LA(1)<=EDGE_OPERATOR_ARROW))
				{
					input.Consume();
					state.errorRecovery=false;
				}
				else
				{
					MismatchedSetException mse = new MismatchedSetException(null,input);
					DebugRecognitionException(mse);
					throw mse;
				}

				DebugLocation(40, 69);
				PushFollow(Follow._id_in_stmt237);
				dest_id=id();
				PopFollow();

				DebugLocation(40, 85);
				PushFollow(Follow._opt_attr_list_in_stmt243);
				edge_attrs=opt_attr_list();
				PopFollow();

				DebugLocation(41, 3);
				 this.AddEdge(source_id, dest_id, edge_attrs); 

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:42:2: NODE opt_attr_list
				{
				DebugLocation(42, 2);
				Match(input,NODE,Follow._NODE_in_stmt253); 
				DebugLocation(42, 7);
				PushFollow(Follow._opt_attr_list_in_stmt255);
				opt_attr_list();
				PopFollow();


				}
				break;
			case 5:
				DebugEnterAlt(5);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:43:2: EDGE opt_attr_list
				{
				DebugLocation(43, 2);
				Match(input,EDGE,Follow._EDGE_in_stmt260); 
				DebugLocation(43, 7);
				PushFollow(Follow._opt_attr_list_in_stmt262);
				opt_attr_list();
				PopFollow();


				}
				break;
			case 6:
				DebugEnterAlt(6);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:43:22: 
				{
				}
				break;

			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("stmt", 5);
			LeaveRule("stmt", 5);
			Leave_stmt();
		}
		DebugLocation(43, 21);
		} finally { DebugExitRule(GrammarFileName, "stmt"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return;

	}
	// $ANTLR end "stmt"


	partial void Enter_subgraph_stmt();
	partial void Leave_subgraph_stmt();

	// $ANTLR start "subgraph_stmt"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:45:1: subgraph_stmt : SUBGRAPH sub_graph_id= id L_CURL_BRACKET stmt_list R_CURL_BRACKET ;
	[GrammarRule("subgraph_stmt")]
	private void subgraph_stmt()
	{
		Enter_subgraph_stmt();
		EnterRule("subgraph_stmt", 6);
		TraceIn("subgraph_stmt", 6);
		string sub_graph_id = default(string);

		try { DebugEnterRule(GrammarFileName, "subgraph_stmt");
		DebugLocation(45, 133);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:46:2: ( SUBGRAPH sub_graph_id= id L_CURL_BRACKET stmt_list R_CURL_BRACKET )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:46:4: SUBGRAPH sub_graph_id= id L_CURL_BRACKET stmt_list R_CURL_BRACKET
			{
			DebugLocation(46, 4);
			Match(input,SUBGRAPH,Follow._SUBGRAPH_in_subgraph_stmt276); 
			DebugLocation(46, 26);
			PushFollow(Follow._id_in_subgraph_stmt282);
			sub_graph_id=id();
			PopFollow();

			DebugLocation(46, 31);
			 this.EnterSubGraph(sub_graph_id); 
			DebugLocation(46, 69);
			Match(input,L_CURL_BRACKET,Follow._L_CURL_BRACKET_in_subgraph_stmt286); 
			DebugLocation(46, 84);
			PushFollow(Follow._stmt_list_in_subgraph_stmt288);
			stmt_list();
			PopFollow();

			DebugLocation(46, 94);
			Match(input,R_CURL_BRACKET,Follow._R_CURL_BRACKET_in_subgraph_stmt290); 
			DebugLocation(46, 109);
			 this.LeaveSubGraph(); 

			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("subgraph_stmt", 6);
			LeaveRule("subgraph_stmt", 6);
			Leave_subgraph_stmt();
		}
		DebugLocation(46, 133);
		} finally { DebugExitRule(GrammarFileName, "subgraph_stmt"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return;

	}
	// $ANTLR end "subgraph_stmt"


	partial void Enter_opt_attr_list();
	partial void Leave_opt_attr_list();

	// $ANTLR start "opt_attr_list"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:48:1: opt_attr_list returns [IDictionary<string, string> opt_attr_list_result] : ( L_SQUARE_BRACKET opt_attr_list_result_value= attr_list R_SQUARE_BRACKET | L_SQUARE_BRACKET R_SQUARE_BRACKET | );
	[GrammarRule("opt_attr_list")]
	private IDictionary<string, string> opt_attr_list()
	{
		Enter_opt_attr_list();
		EnterRule("opt_attr_list", 7);
		TraceIn("opt_attr_list", 7);
		IDictionary<string, string> opt_attr_list_result = default(IDictionary<string, string>);

		IDictionary<string, string> opt_attr_list_result_value = default(IDictionary<string, string>);

		try { DebugEnterRule(GrammarFileName, "opt_attr_list");
		DebugLocation(48, 62);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:49:2: ( L_SQUARE_BRACKET opt_attr_list_result_value= attr_list R_SQUARE_BRACKET | L_SQUARE_BRACKET R_SQUARE_BRACKET | )
			int alt4=3;
			try { DebugEnterDecision(4, decisionCanBacktrack[4]);
			int LA4_0 = input.LA(1);

			if ((LA4_0==L_SQUARE_BRACKET))
			{
				int LA4_1 = input.LA(2);

				if ((LA4_1==R_SQUARE_BRACKET))
				{
					alt4=2;
				}
				else if ((LA4_1==ID))
				{
					alt4=1;
				}
				else
				{
					NoViableAltException nvae = new NoViableAltException("", 4, 1, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}
			else if ((LA4_0==SEMICOLON))
			{
				alt4=3;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 4, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(4); }
			switch (alt4)
			{
			case 1:
				DebugEnterAlt(1);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:49:4: L_SQUARE_BRACKET opt_attr_list_result_value= attr_list R_SQUARE_BRACKET
				{
				DebugLocation(49, 4);
				Match(input,L_SQUARE_BRACKET,Follow._L_SQUARE_BRACKET_in_opt_attr_list305); 
				DebugLocation(49, 48);
				PushFollow(Follow._attr_list_in_opt_attr_list311);
				opt_attr_list_result_value=attr_list();
				PopFollow();

				DebugLocation(49, 60);
				Match(input,R_SQUARE_BRACKET,Follow._R_SQUARE_BRACKET_in_opt_attr_list313); 
				DebugLocation(49, 77);
				 opt_attr_list_result = opt_attr_list_result_value; 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:50:4: L_SQUARE_BRACKET R_SQUARE_BRACKET
				{
				DebugLocation(50, 4);
				Match(input,L_SQUARE_BRACKET,Follow._L_SQUARE_BRACKET_in_opt_attr_list322); 
				DebugLocation(50, 21);
				Match(input,R_SQUARE_BRACKET,Follow._R_SQUARE_BRACKET_in_opt_attr_list324); 
				DebugLocation(50, 38);
				 opt_attr_list_result = new Dictionary<string, string>(); 

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:51:3: 
				{
				DebugLocation(51, 3);
				 opt_attr_list_result = new Dictionary<string, string>(); 

				}
				break;

			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("opt_attr_list", 7);
			LeaveRule("opt_attr_list", 7);
			Leave_opt_attr_list();
		}
		DebugLocation(51, 62);
		} finally { DebugExitRule(GrammarFileName, "opt_attr_list"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return opt_attr_list_result;

	}
	// $ANTLR end "opt_attr_list"


	partial void Enter_attr_list();
	partial void Leave_attr_list();

	// $ANTLR start "attr_list"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:53:1: attr_list returns [IDictionary<string, string> attr_list_result] : attr_list_attr= attr ( COMMA attr_list_attr2= attr )* ;
	[GrammarRule("attr_list")]
	private IDictionary<string, string> attr_list()
	{
		Enter_attr_list();
		EnterRule("attr_list", 8);
		TraceIn("attr_list", 8);
		IDictionary<string, string> attr_list_result = default(IDictionary<string, string>);

		KeyValuePair<string, string> attr_list_attr = default(KeyValuePair<string, string>);
		KeyValuePair<string, string> attr_list_attr2 = default(KeyValuePair<string, string>);

		 attr_list_result = new Dictionary<string, string>(); 
		try { DebugEnterRule(GrammarFileName, "attr_list");
		DebugLocation(53, 77);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:55:2: (attr_list_attr= attr ( COMMA attr_list_attr2= attr )* )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:55:4: attr_list_attr= attr ( COMMA attr_list_attr2= attr )*
			{
			DebugLocation(55, 19);
			PushFollow(Follow._attr_in_attr_list356);
			attr_list_attr=attr();
			PopFollow();

			DebugLocation(55, 26);
			 attr_list_result.Add(attr_list_attr); 
			DebugLocation(56, 4);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:56:4: ( COMMA attr_list_attr2= attr )*
			try { DebugEnterSubRule(5);
			while (true)
			{
				int alt5=2;
				try { DebugEnterDecision(5, decisionCanBacktrack[5]);
				int LA5_0 = input.LA(1);

				if ((LA5_0==COMMA))
				{
					alt5=1;
				}


				} finally { DebugExitDecision(5); }
				switch ( alt5 )
				{
				case 1:
					DebugEnterAlt(1);
					// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:56:5: COMMA attr_list_attr2= attr
					{
					DebugLocation(56, 5);
					Match(input,COMMA,Follow._COMMA_in_attr_list364); 
					DebugLocation(56, 27);
					PushFollow(Follow._attr_in_attr_list370);
					attr_list_attr2=attr();
					PopFollow();

					DebugLocation(56, 34);
					 attr_list_result.Add(attr_list_attr2); 

					}
					break;

				default:
					goto loop5;
				}
			}

			loop5:
				;

			} finally { DebugExitSubRule(5); }


			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("attr_list", 8);
			LeaveRule("attr_list", 8);
			Leave_attr_list();
		}
		DebugLocation(56, 77);
		} finally { DebugExitRule(GrammarFileName, "attr_list"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return attr_list_result;

	}
	// $ANTLR end "attr_list"


	partial void Enter_attr();
	partial void Leave_attr();

	// $ANTLR start "attr"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:58:1: attr returns [KeyValuePair<string, string> attr_result] : attr_result_id= id EQUALS attr_result_value= attr_value ;
	[GrammarRule("attr")]
	private KeyValuePair<string, string> attr()
	{
		Enter_attr();
		EnterRule("attr", 9);
		TraceIn("attr", 9);
		KeyValuePair<string, string> attr_result = default(KeyValuePair<string, string>);

		string attr_result_id = default(string);
		string attr_result_value = default(string);

		try { DebugEnterRule(GrammarFileName, "attr");
		DebugLocation(58, 87);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:59:2: (attr_result_id= id EQUALS attr_result_value= attr_value )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:59:4: attr_result_id= id EQUALS attr_result_value= attr_value
			{
			DebugLocation(59, 19);
			PushFollow(Follow._id_in_attr391);
			attr_result_id=id();
			PopFollow();

			DebugLocation(59, 24);
			Match(input,EQUALS,Follow._EQUALS_in_attr393); 
			DebugLocation(59, 49);
			PushFollow(Follow._attr_value_in_attr399);
			attr_result_value=attr_value();
			PopFollow();

			DebugLocation(60, 2);
			 attr_result = new KeyValuePair<string, string>(attr_result_id, attr_result_value); 

			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("attr", 9);
			LeaveRule("attr", 9);
			Leave_attr();
		}
		DebugLocation(60, 87);
		} finally { DebugExitRule(GrammarFileName, "attr"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return attr_result;

	}
	// $ANTLR end "attr"


	partial void Enter_attr_value();
	partial void Leave_attr_value();

	// $ANTLR start "attr_value"
	// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:62:1: attr_value returns [string attr_value_result] : (attr_value_result_value= ID | attr_value_quoted= QUOTED_VALUE );
	[GrammarRule("attr_value")]
	private string attr_value()
	{
		Enter_attr_value();
		EnterRule("attr_value", 10);
		TraceIn("attr_value", 10);
		string attr_value_result = default(string);

		IToken attr_value_result_value=null;
		IToken attr_value_quoted=null;

		try { DebugEnterRule(GrammarFileName, "attr_value");
		DebugLocation(62, 97);
		if (RuleLevel == 0)
			DebugListener.Commence();
		IncRuleLevel();
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:63:2: (attr_value_result_value= ID | attr_value_quoted= QUOTED_VALUE )
			int alt6=2;
			try { DebugEnterDecision(6, decisionCanBacktrack[6]);
			int LA6_0 = input.LA(1);

			if ((LA6_0==ID))
			{
				alt6=1;
			}
			else if ((LA6_0==QUOTED_VALUE))
			{
				alt6=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 6, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(6); }
			switch (alt6)
			{
			case 1:
				DebugEnterAlt(1);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:63:4: attr_value_result_value= ID
				{
				DebugLocation(63, 28);
				attr_value_result_value=(IToken)Match(input,ID,Follow._ID_in_attr_value421); 
				DebugLocation(63, 33);
				 attr_value_result = attr_value_result_value.Text; 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:64:4: attr_value_quoted= QUOTED_VALUE
				{
				DebugLocation(64, 22);
				attr_value_quoted=(IToken)Match(input,QUOTED_VALUE,Follow._QUOTED_VALUE_in_attr_value434); 
				DebugLocation(64, 37);
				 attr_value_result = this.Unquote(attr_value_quoted.Text); 

				}
				break;

			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("attr_value", 10);
			LeaveRule("attr_value", 10);
			Leave_attr_value();
		}
		DebugLocation(64, 97);
		} finally { DebugExitRule(GrammarFileName, "attr_value"); }
		DecRuleLevel();
		if (RuleLevel == 0)
			DebugListener.Terminate();
		return attr_value_result;

	}
	// $ANTLR end "attr_value"
	#endregion Rules


	#region Follow sets
	private static class Follow
	{
		public static readonly BitSet _GRAPH_in_dot116 = new BitSet(new ulong[]{0x1010UL});
		public static readonly BitSet _opt_id_in_dot118 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _L_CURL_BRACKET_in_dot120 = new BitSet(new ulong[]{0x39920UL});
		public static readonly BitSet _stmt_list_in_dot122 = new BitSet(new ulong[]{0x20UL});
		public static readonly BitSet _R_CURL_BRACKET_in_dot124 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ID_in_id140 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_opt_id149 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _subgraph_stmt_in_stmt_list163 = new BitSet(new ulong[]{0x39900UL});
		public static readonly BitSet _stmt_list_in_stmt_list165 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _stmt_in_stmt_list171 = new BitSet(new ulong[]{0x100UL});
		public static readonly BitSet _SEMICOLON_in_stmt_list173 = new BitSet(new ulong[]{0x39900UL});
		public static readonly BitSet _stmt_list_in_stmt_list175 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _GRAPH_in_stmt191 = new BitSet(new ulong[]{0x40UL});
		public static readonly BitSet _opt_attr_list_in_stmt197 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_stmt208 = new BitSet(new ulong[]{0x40UL});
		public static readonly BitSet _opt_attr_list_in_stmt214 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _id_in_stmt225 = new BitSet(new ulong[]{0x6000UL});
		public static readonly BitSet _set_in_stmt227 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _id_in_stmt237 = new BitSet(new ulong[]{0x40UL});
		public static readonly BitSet _opt_attr_list_in_stmt243 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NODE_in_stmt253 = new BitSet(new ulong[]{0x40UL});
		public static readonly BitSet _opt_attr_list_in_stmt255 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _EDGE_in_stmt260 = new BitSet(new ulong[]{0x40UL});
		public static readonly BitSet _opt_attr_list_in_stmt262 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SUBGRAPH_in_subgraph_stmt276 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _id_in_subgraph_stmt282 = new BitSet(new ulong[]{0x10UL});
		public static readonly BitSet _L_CURL_BRACKET_in_subgraph_stmt286 = new BitSet(new ulong[]{0x39920UL});
		public static readonly BitSet _stmt_list_in_subgraph_stmt288 = new BitSet(new ulong[]{0x20UL});
		public static readonly BitSet _R_CURL_BRACKET_in_subgraph_stmt290 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _L_SQUARE_BRACKET_in_opt_attr_list305 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _attr_list_in_opt_attr_list311 = new BitSet(new ulong[]{0x80UL});
		public static readonly BitSet _R_SQUARE_BRACKET_in_opt_attr_list313 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _L_SQUARE_BRACKET_in_opt_attr_list322 = new BitSet(new ulong[]{0x80UL});
		public static readonly BitSet _R_SQUARE_BRACKET_in_opt_attr_list324 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _attr_in_attr_list356 = new BitSet(new ulong[]{0x402UL});
		public static readonly BitSet _COMMA_in_attr_list364 = new BitSet(new ulong[]{0x1000UL});
		public static readonly BitSet _attr_in_attr_list370 = new BitSet(new ulong[]{0x402UL});
		public static readonly BitSet _id_in_attr391 = new BitSet(new ulong[]{0x200UL});
		public static readonly BitSet _EQUALS_in_attr393 = new BitSet(new ulong[]{0x41000UL});
		public static readonly BitSet _attr_value_in_attr399 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ID_in_attr_value421 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _QUOTED_VALUE_in_attr_value434 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace Graphviz4Net.Dot.AntlrParser

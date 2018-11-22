// $ANTLR 3.3 Nov 30, 2010 12:45:30 D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g 2013-05-05 00:38:30

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

namespace Graphviz4Net.Dot.AntlrParser
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3 Nov 30, 2010 12:45:30")]
[System.CLSCompliant(false)]
public partial class DotGrammarLexer : Antlr.Runtime.Lexer
{
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

	public DotGrammarLexer()
	{
		OnCreated();
	}

	public DotGrammarLexer(ICharStream input )
		: this(input, new RecognizerSharedState())
	{
	}

	public DotGrammarLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state)
	{


		OnCreated();
	}
	public override string GrammarFileName { get { return "D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g"; } }

	private static readonly bool[] decisionCanBacktrack = new bool[0];


	partial void OnCreated();
	partial void EnterRule(string ruleName, int ruleIndex);
	partial void LeaveRule(string ruleName, int ruleIndex);

	partial void Enter_L_CURL_BRACKET();
	partial void Leave_L_CURL_BRACKET();

	// $ANTLR start "L_CURL_BRACKET"
	[GrammarRule("L_CURL_BRACKET")]
	private void mL_CURL_BRACKET()
	{
		Enter_L_CURL_BRACKET();
		EnterRule("L_CURL_BRACKET", 1);
		TraceIn("L_CURL_BRACKET", 1);
		try
		{
			int _type = L_CURL_BRACKET;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:9:16: ( '{' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:9:18: '{'
			{
			DebugLocation(9, 18);
			Match('{'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("L_CURL_BRACKET", 1);
			LeaveRule("L_CURL_BRACKET", 1);
			Leave_L_CURL_BRACKET();
		}
	}
	// $ANTLR end "L_CURL_BRACKET"

	partial void Enter_R_CURL_BRACKET();
	partial void Leave_R_CURL_BRACKET();

	// $ANTLR start "R_CURL_BRACKET"
	[GrammarRule("R_CURL_BRACKET")]
	private void mR_CURL_BRACKET()
	{
		Enter_R_CURL_BRACKET();
		EnterRule("R_CURL_BRACKET", 2);
		TraceIn("R_CURL_BRACKET", 2);
		try
		{
			int _type = R_CURL_BRACKET;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:10:16: ( '}' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:10:18: '}'
			{
			DebugLocation(10, 18);
			Match('}'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("R_CURL_BRACKET", 2);
			LeaveRule("R_CURL_BRACKET", 2);
			Leave_R_CURL_BRACKET();
		}
	}
	// $ANTLR end "R_CURL_BRACKET"

	partial void Enter_L_SQUARE_BRACKET();
	partial void Leave_L_SQUARE_BRACKET();

	// $ANTLR start "L_SQUARE_BRACKET"
	[GrammarRule("L_SQUARE_BRACKET")]
	private void mL_SQUARE_BRACKET()
	{
		Enter_L_SQUARE_BRACKET();
		EnterRule("L_SQUARE_BRACKET", 3);
		TraceIn("L_SQUARE_BRACKET", 3);
		try
		{
			int _type = L_SQUARE_BRACKET;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:11:18: ( '[' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:11:20: '['
			{
			DebugLocation(11, 20);
			Match('['); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("L_SQUARE_BRACKET", 3);
			LeaveRule("L_SQUARE_BRACKET", 3);
			Leave_L_SQUARE_BRACKET();
		}
	}
	// $ANTLR end "L_SQUARE_BRACKET"

	partial void Enter_R_SQUARE_BRACKET();
	partial void Leave_R_SQUARE_BRACKET();

	// $ANTLR start "R_SQUARE_BRACKET"
	[GrammarRule("R_SQUARE_BRACKET")]
	private void mR_SQUARE_BRACKET()
	{
		Enter_R_SQUARE_BRACKET();
		EnterRule("R_SQUARE_BRACKET", 4);
		TraceIn("R_SQUARE_BRACKET", 4);
		try
		{
			int _type = R_SQUARE_BRACKET;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:12:18: ( ']' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:12:20: ']'
			{
			DebugLocation(12, 20);
			Match(']'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("R_SQUARE_BRACKET", 4);
			LeaveRule("R_SQUARE_BRACKET", 4);
			Leave_R_SQUARE_BRACKET();
		}
	}
	// $ANTLR end "R_SQUARE_BRACKET"

	partial void Enter_SEMICOLON();
	partial void Leave_SEMICOLON();

	// $ANTLR start "SEMICOLON"
	[GrammarRule("SEMICOLON")]
	private void mSEMICOLON()
	{
		Enter_SEMICOLON();
		EnterRule("SEMICOLON", 5);
		TraceIn("SEMICOLON", 5);
		try
		{
			int _type = SEMICOLON;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:13:11: ( ';' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:13:13: ';'
			{
			DebugLocation(13, 13);
			Match(';'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("SEMICOLON", 5);
			LeaveRule("SEMICOLON", 5);
			Leave_SEMICOLON();
		}
	}
	// $ANTLR end "SEMICOLON"

	partial void Enter_EQUALS();
	partial void Leave_EQUALS();

	// $ANTLR start "EQUALS"
	[GrammarRule("EQUALS")]
	private void mEQUALS()
	{
		Enter_EQUALS();
		EnterRule("EQUALS", 6);
		TraceIn("EQUALS", 6);
		try
		{
			int _type = EQUALS;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:14:8: ( '=' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:14:10: '='
			{
			DebugLocation(14, 10);
			Match('='); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("EQUALS", 6);
			LeaveRule("EQUALS", 6);
			Leave_EQUALS();
		}
	}
	// $ANTLR end "EQUALS"

	partial void Enter_COMMA();
	partial void Leave_COMMA();

	// $ANTLR start "COMMA"
	[GrammarRule("COMMA")]
	private void mCOMMA()
	{
		Enter_COMMA();
		EnterRule("COMMA", 7);
		TraceIn("COMMA", 7);
		try
		{
			int _type = COMMA;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:15:7: ( ',' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:15:9: ','
			{
			DebugLocation(15, 9);
			Match(','); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("COMMA", 7);
			LeaveRule("COMMA", 7);
			Leave_COMMA();
		}
	}
	// $ANTLR end "COMMA"

	partial void Enter_QUOTED_VALUE();
	partial void Leave_QUOTED_VALUE();

	// $ANTLR start "QUOTED_VALUE"
	[GrammarRule("QUOTED_VALUE")]
	private void mQUOTED_VALUE()
	{
		Enter_QUOTED_VALUE();
		EnterRule("QUOTED_VALUE", 8);
		TraceIn("QUOTED_VALUE", 8);
		try
		{
			int _type = QUOTED_VALUE;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:67:14: ( '\"' (~ '\"' )* '\"' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:67:16: '\"' (~ '\"' )* '\"'
			{
			DebugLocation(67, 16);
			Match('\"'); 
			DebugLocation(67, 20);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:67:20: (~ '\"' )*
			try { DebugEnterSubRule(1);
			while (true)
			{
				int alt1=2;
				try { DebugEnterDecision(1, decisionCanBacktrack[1]);
				int LA1_0 = input.LA(1);

				if (((LA1_0>='\u0000' && LA1_0<='!')||(LA1_0>='#' && LA1_0<='\uFFFF')))
				{
					alt1=1;
				}


				} finally { DebugExitDecision(1); }
				switch ( alt1 )
				{
				case 1:
					DebugEnterAlt(1);
					// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:67:21: ~ '\"'
					{
					DebugLocation(67, 21);
					if ((input.LA(1)>='\u0000' && input.LA(1)<='!')||(input.LA(1)>='#' && input.LA(1)<='\uFFFF'))
					{
						input.Consume();

					}
					else
					{
						MismatchedSetException mse = new MismatchedSetException(null,input);
						DebugRecognitionException(mse);
						Recover(mse);
						throw mse;}


					}
					break;

				default:
					goto loop1;
				}
			}

			loop1:
				;

			} finally { DebugExitSubRule(1); }

			DebugLocation(67, 28);
			Match('\"'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("QUOTED_VALUE", 8);
			LeaveRule("QUOTED_VALUE", 8);
			Leave_QUOTED_VALUE();
		}
	}
	// $ANTLR end "QUOTED_VALUE"

	partial void Enter_SUBGRAPH();
	partial void Leave_SUBGRAPH();

	// $ANTLR start "SUBGRAPH"
	[GrammarRule("SUBGRAPH")]
	private void mSUBGRAPH()
	{
		Enter_SUBGRAPH();
		EnterRule("SUBGRAPH", 9);
		TraceIn("SUBGRAPH", 9);
		try
		{
			int _type = SUBGRAPH;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:70:9: ( S U B G R A P H )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:70:11: S U B G R A P H
			{
			DebugLocation(70, 11);
			mS(); 
			DebugLocation(70, 13);
			mU(); 
			DebugLocation(70, 15);
			mB(); 
			DebugLocation(70, 17);
			mG(); 
			DebugLocation(70, 19);
			mR(); 
			DebugLocation(70, 21);
			mA(); 
			DebugLocation(70, 23);
			mP(); 
			DebugLocation(70, 25);
			mH(); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("SUBGRAPH", 9);
			LeaveRule("SUBGRAPH", 9);
			Leave_SUBGRAPH();
		}
	}
	// $ANTLR end "SUBGRAPH"

	partial void Enter_GRAPH();
	partial void Leave_GRAPH();

	// $ANTLR start "GRAPH"
	[GrammarRule("GRAPH")]
	private void mGRAPH()
	{
		Enter_GRAPH();
		EnterRule("GRAPH", 10);
		TraceIn("GRAPH", 10);
		try
		{
			int _type = GRAPH;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:71:7: ( ( G R A P H | D I G R A P H ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:71:9: ( G R A P H | D I G R A P H )
			{
			DebugLocation(71, 9);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:71:9: ( G R A P H | D I G R A P H )
			int alt2=2;
			try { DebugEnterSubRule(2);
			try { DebugEnterDecision(2, decisionCanBacktrack[2]);
			int LA2_0 = input.LA(1);

			if ((LA2_0=='G'||LA2_0=='g'))
			{
				alt2=1;
			}
			else if ((LA2_0=='D'||LA2_0=='d'))
			{
				alt2=2;
			}
			else
			{
				NoViableAltException nvae = new NoViableAltException("", 2, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(2); }
			switch (alt2)
			{
			case 1:
				DebugEnterAlt(1);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:71:10: G R A P H
				{
				DebugLocation(71, 10);
				mG(); 
				DebugLocation(71, 12);
				mR(); 
				DebugLocation(71, 14);
				mA(); 
				DebugLocation(71, 16);
				mP(); 
				DebugLocation(71, 18);
				mH(); 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:71:20: D I G R A P H
				{
				DebugLocation(71, 20);
				mD(); 
				DebugLocation(71, 22);
				mI(); 
				DebugLocation(71, 24);
				mG(); 
				DebugLocation(71, 26);
				mR(); 
				DebugLocation(71, 28);
				mA(); 
				DebugLocation(71, 30);
				mP(); 
				DebugLocation(71, 32);
				mH(); 

				}
				break;

			}
			} finally { DebugExitSubRule(2); }


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("GRAPH", 10);
			LeaveRule("GRAPH", 10);
			Leave_GRAPH();
		}
	}
	// $ANTLR end "GRAPH"

	partial void Enter_NODE();
	partial void Leave_NODE();

	// $ANTLR start "NODE"
	[GrammarRule("NODE")]
	private void mNODE()
	{
		Enter_NODE();
		EnterRule("NODE", 11);
		TraceIn("NODE", 11);
		try
		{
			int _type = NODE;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:72:6: ( N O D E )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:72:8: N O D E
			{
			DebugLocation(72, 8);
			mN(); 
			DebugLocation(72, 10);
			mO(); 
			DebugLocation(72, 12);
			mD(); 
			DebugLocation(72, 14);
			mE(); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("NODE", 11);
			LeaveRule("NODE", 11);
			Leave_NODE();
		}
	}
	// $ANTLR end "NODE"

	partial void Enter_EDGE();
	partial void Leave_EDGE();

	// $ANTLR start "EDGE"
	[GrammarRule("EDGE")]
	private void mEDGE()
	{
		Enter_EDGE();
		EnterRule("EDGE", 12);
		TraceIn("EDGE", 12);
		try
		{
			int _type = EDGE;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:73:6: ( E D G E )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:73:8: E D G E
			{
			DebugLocation(73, 8);
			mE(); 
			DebugLocation(73, 10);
			mD(); 
			DebugLocation(73, 12);
			mG(); 
			DebugLocation(73, 14);
			mE(); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("EDGE", 12);
			LeaveRule("EDGE", 12);
			Leave_EDGE();
		}
	}
	// $ANTLR end "EDGE"

	partial void Enter_EDGE_OPERATOR_NOARROW();
	partial void Leave_EDGE_OPERATOR_NOARROW();

	// $ANTLR start "EDGE_OPERATOR_NOARROW"
	[GrammarRule("EDGE_OPERATOR_NOARROW")]
	private void mEDGE_OPERATOR_NOARROW()
	{
		Enter_EDGE_OPERATOR_NOARROW();
		EnterRule("EDGE_OPERATOR_NOARROW", 13);
		TraceIn("EDGE_OPERATOR_NOARROW", 13);
		try
		{
			int _type = EDGE_OPERATOR_NOARROW;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:74:23: ( '--' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:74:25: '--'
			{
			DebugLocation(74, 25);
			Match("--"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("EDGE_OPERATOR_NOARROW", 13);
			LeaveRule("EDGE_OPERATOR_NOARROW", 13);
			Leave_EDGE_OPERATOR_NOARROW();
		}
	}
	// $ANTLR end "EDGE_OPERATOR_NOARROW"

	partial void Enter_EDGE_OPERATOR_ARROW();
	partial void Leave_EDGE_OPERATOR_ARROW();

	// $ANTLR start "EDGE_OPERATOR_ARROW"
	[GrammarRule("EDGE_OPERATOR_ARROW")]
	private void mEDGE_OPERATOR_ARROW()
	{
		Enter_EDGE_OPERATOR_ARROW();
		EnterRule("EDGE_OPERATOR_ARROW", 14);
		TraceIn("EDGE_OPERATOR_ARROW", 14);
		try
		{
			int _type = EDGE_OPERATOR_ARROW;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:75:21: ( '->' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:75:23: '->'
			{
			DebugLocation(75, 23);
			Match("->"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("EDGE_OPERATOR_ARROW", 14);
			LeaveRule("EDGE_OPERATOR_ARROW", 14);
			Leave_EDGE_OPERATOR_ARROW();
		}
	}
	// $ANTLR end "EDGE_OPERATOR_ARROW"

	partial void Enter_ALLOWED_QUOTED_VALUES();
	partial void Leave_ALLOWED_QUOTED_VALUES();

	// $ANTLR start "ALLOWED_QUOTED_VALUES"
	[GrammarRule("ALLOWED_QUOTED_VALUES")]
	private void mALLOWED_QUOTED_VALUES()
	{
		Enter_ALLOWED_QUOTED_VALUES();
		EnterRule("ALLOWED_QUOTED_VALUES", 15);
		TraceIn("ALLOWED_QUOTED_VALUES", 15);
		try
		{
			int _type = ALLOWED_QUOTED_VALUES;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:78:23: ( ( '.' )+ )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:78:25: ( '.' )+
			{
			DebugLocation(78, 25);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:78:25: ( '.' )+
			int cnt3=0;
			try { DebugEnterSubRule(3);
			while (true)
			{
				int alt3=2;
				try { DebugEnterDecision(3, decisionCanBacktrack[3]);
				int LA3_0 = input.LA(1);

				if ((LA3_0=='.'))
				{
					alt3=1;
				}


				} finally { DebugExitDecision(3); }
				switch (alt3)
				{
				case 1:
					DebugEnterAlt(1);
					// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:78:26: '.'
					{
					DebugLocation(78, 26);
					Match('.'); 

					}
					break;

				default:
					if (cnt3 >= 1)
						goto loop3;

					EarlyExitException eee3 = new EarlyExitException( 3, input );
					DebugRecognitionException(eee3);
					throw eee3;
				}
				cnt3++;
			}
			loop3:
				;

			} finally { DebugExitSubRule(3); }


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("ALLOWED_QUOTED_VALUES", 15);
			LeaveRule("ALLOWED_QUOTED_VALUES", 15);
			Leave_ALLOWED_QUOTED_VALUES();
		}
	}
	// $ANTLR end "ALLOWED_QUOTED_VALUES"

	partial void Enter_ID();
	partial void Leave_ID();

	// $ANTLR start "ID"
	[GrammarRule("ID")]
	private void mID()
	{
		Enter_ID();
		EnterRule("ID", 16);
		TraceIn("ID", 16);
		try
		{
			int _type = ID;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:79:5: ( ( STR | NUMBER | '_' | ':' )+ )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:79:9: ( STR | NUMBER | '_' | ':' )+
			{
			DebugLocation(79, 9);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:79:9: ( STR | NUMBER | '_' | ':' )+
			int cnt4=0;
			try { DebugEnterSubRule(4);
			while (true)
			{
				int alt4=2;
				try { DebugEnterDecision(4, decisionCanBacktrack[4]);
				int LA4_0 = input.LA(1);

				if (((LA4_0>='0' && LA4_0<=':')||(LA4_0>='A' && LA4_0<='Z')||LA4_0=='_'||(LA4_0>='a' && LA4_0<='z')))
				{
					alt4=1;
				}


				} finally { DebugExitDecision(4); }
				switch (alt4)
				{
				case 1:
					DebugEnterAlt(1);
					// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:
					{
					DebugLocation(79, 9);
					if ((input.LA(1)>='0' && input.LA(1)<=':')||(input.LA(1)>='A' && input.LA(1)<='Z')||input.LA(1)=='_'||(input.LA(1)>='a' && input.LA(1)<='z'))
					{
						input.Consume();

					}
					else
					{
						MismatchedSetException mse = new MismatchedSetException(null,input);
						DebugRecognitionException(mse);
						Recover(mse);
						throw mse;}


					}
					break;

				default:
					if (cnt4 >= 1)
						goto loop4;

					EarlyExitException eee4 = new EarlyExitException( 4, input );
					DebugRecognitionException(eee4);
					throw eee4;
				}
				cnt4++;
			}
			loop4:
				;

			} finally { DebugExitSubRule(4); }


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("ID", 16);
			LeaveRule("ID", 16);
			Leave_ID();
		}
	}
	// $ANTLR end "ID"

	partial void Enter_STR();
	partial void Leave_STR();

	// $ANTLR start "STR"
	[GrammarRule("STR")]
	private void mSTR()
	{
		Enter_STR();
		EnterRule("STR", 17);
		TraceIn("STR", 17);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:80:14: ( ( 'a' .. 'z' | 'A' .. 'Z' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:80:16: ( 'a' .. 'z' | 'A' .. 'Z' )
			{
			DebugLocation(80, 16);
			if ((input.LA(1)>='A' && input.LA(1)<='Z')||(input.LA(1)>='a' && input.LA(1)<='z'))
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("STR", 17);
			LeaveRule("STR", 17);
			Leave_STR();
		}
	}
	// $ANTLR end "STR"

	partial void Enter_NUMBER();
	partial void Leave_NUMBER();

	// $ANTLR start "NUMBER"
	[GrammarRule("NUMBER")]
	private void mNUMBER()
	{
		Enter_NUMBER();
		EnterRule("NUMBER", 18);
		TraceIn("NUMBER", 18);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:81:17: ( '0' .. '9' )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:81:19: '0' .. '9'
			{
			DebugLocation(81, 19);
			MatchRange('0','9'); 

			}

		}
		finally
		{
			TraceOut("NUMBER", 18);
			LeaveRule("NUMBER", 18);
			Leave_NUMBER();
		}
	}
	// $ANTLR end "NUMBER"

	partial void Enter_WS();
	partial void Leave_WS();

	// $ANTLR start "WS"
	[GrammarRule("WS")]
	private void mWS()
	{
		Enter_WS();
		EnterRule("WS", 19);
		TraceIn("WS", 19);
		try
		{
			int _type = WS;
			int _channel = DefaultTokenChannel;
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:84:5: ( ( ' ' | '\\t' | '\\r' | '\\n' )+ )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:84:9: ( ' ' | '\\t' | '\\r' | '\\n' )+
			{
			DebugLocation(84, 9);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:84:9: ( ' ' | '\\t' | '\\r' | '\\n' )+
			int cnt5=0;
			try { DebugEnterSubRule(5);
			while (true)
			{
				int alt5=2;
				try { DebugEnterDecision(5, decisionCanBacktrack[5]);
				int LA5_0 = input.LA(1);

				if (((LA5_0>='\t' && LA5_0<='\n')||LA5_0=='\r'||LA5_0==' '))
				{
					alt5=1;
				}


				} finally { DebugExitDecision(5); }
				switch (alt5)
				{
				case 1:
					DebugEnterAlt(1);
					// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:
					{
					DebugLocation(84, 9);
					if ((input.LA(1)>='\t' && input.LA(1)<='\n')||input.LA(1)=='\r'||input.LA(1)==' ')
					{
						input.Consume();

					}
					else
					{
						MismatchedSetException mse = new MismatchedSetException(null,input);
						DebugRecognitionException(mse);
						Recover(mse);
						throw mse;}


					}
					break;

				default:
					if (cnt5 >= 1)
						goto loop5;

					EarlyExitException eee5 = new EarlyExitException( 5, input );
					DebugRecognitionException(eee5);
					throw eee5;
				}
				cnt5++;
			}
			loop5:
				;

			} finally { DebugExitSubRule(5); }

			DebugLocation(84, 31);

			#if SOMETHING_THAT_DOES_NOT_EXIST
			_channel=HIDDEN;	// antlr works does not recognize C# constant Hidden
			#else
			_channel=Hidden;
			#endif


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
			TraceOut("WS", 19);
			LeaveRule("WS", 19);
			Leave_WS();
		}
	}
	// $ANTLR end "WS"

	partial void Enter_A();
	partial void Leave_A();

	// $ANTLR start "A"
	[GrammarRule("A")]
	private void mA()
	{
		Enter_A();
		EnterRule("A", 20);
		TraceIn("A", 20);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:93:11: ( ( 'a' | 'A' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:93:12: ( 'a' | 'A' )
			{
			DebugLocation(93, 12);
			if (input.LA(1)=='A'||input.LA(1)=='a')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("A", 20);
			LeaveRule("A", 20);
			Leave_A();
		}
	}
	// $ANTLR end "A"

	partial void Enter_B();
	partial void Leave_B();

	// $ANTLR start "B"
	[GrammarRule("B")]
	private void mB()
	{
		Enter_B();
		EnterRule("B", 21);
		TraceIn("B", 21);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:94:11: ( ( 'b' | 'B' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:94:12: ( 'b' | 'B' )
			{
			DebugLocation(94, 12);
			if (input.LA(1)=='B'||input.LA(1)=='b')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("B", 21);
			LeaveRule("B", 21);
			Leave_B();
		}
	}
	// $ANTLR end "B"

	partial void Enter_C();
	partial void Leave_C();

	// $ANTLR start "C"
	[GrammarRule("C")]
	private void mC()
	{
		Enter_C();
		EnterRule("C", 22);
		TraceIn("C", 22);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:95:11: ( ( 'c' | 'C' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:95:12: ( 'c' | 'C' )
			{
			DebugLocation(95, 12);
			if (input.LA(1)=='C'||input.LA(1)=='c')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("C", 22);
			LeaveRule("C", 22);
			Leave_C();
		}
	}
	// $ANTLR end "C"

	partial void Enter_D();
	partial void Leave_D();

	// $ANTLR start "D"
	[GrammarRule("D")]
	private void mD()
	{
		Enter_D();
		EnterRule("D", 23);
		TraceIn("D", 23);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:96:11: ( ( 'd' | 'D' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:96:12: ( 'd' | 'D' )
			{
			DebugLocation(96, 12);
			if (input.LA(1)=='D'||input.LA(1)=='d')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("D", 23);
			LeaveRule("D", 23);
			Leave_D();
		}
	}
	// $ANTLR end "D"

	partial void Enter_E();
	partial void Leave_E();

	// $ANTLR start "E"
	[GrammarRule("E")]
	private void mE()
	{
		Enter_E();
		EnterRule("E", 24);
		TraceIn("E", 24);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:97:11: ( ( 'e' | 'E' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:97:12: ( 'e' | 'E' )
			{
			DebugLocation(97, 12);
			if (input.LA(1)=='E'||input.LA(1)=='e')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("E", 24);
			LeaveRule("E", 24);
			Leave_E();
		}
	}
	// $ANTLR end "E"

	partial void Enter_F();
	partial void Leave_F();

	// $ANTLR start "F"
	[GrammarRule("F")]
	private void mF()
	{
		Enter_F();
		EnterRule("F", 25);
		TraceIn("F", 25);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:98:11: ( ( 'f' | 'F' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:98:12: ( 'f' | 'F' )
			{
			DebugLocation(98, 12);
			if (input.LA(1)=='F'||input.LA(1)=='f')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("F", 25);
			LeaveRule("F", 25);
			Leave_F();
		}
	}
	// $ANTLR end "F"

	partial void Enter_G();
	partial void Leave_G();

	// $ANTLR start "G"
	[GrammarRule("G")]
	private void mG()
	{
		Enter_G();
		EnterRule("G", 26);
		TraceIn("G", 26);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:99:11: ( ( 'g' | 'G' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:99:12: ( 'g' | 'G' )
			{
			DebugLocation(99, 12);
			if (input.LA(1)=='G'||input.LA(1)=='g')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("G", 26);
			LeaveRule("G", 26);
			Leave_G();
		}
	}
	// $ANTLR end "G"

	partial void Enter_H();
	partial void Leave_H();

	// $ANTLR start "H"
	[GrammarRule("H")]
	private void mH()
	{
		Enter_H();
		EnterRule("H", 27);
		TraceIn("H", 27);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:100:11: ( ( 'h' | 'H' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:100:12: ( 'h' | 'H' )
			{
			DebugLocation(100, 12);
			if (input.LA(1)=='H'||input.LA(1)=='h')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("H", 27);
			LeaveRule("H", 27);
			Leave_H();
		}
	}
	// $ANTLR end "H"

	partial void Enter_I();
	partial void Leave_I();

	// $ANTLR start "I"
	[GrammarRule("I")]
	private void mI()
	{
		Enter_I();
		EnterRule("I", 28);
		TraceIn("I", 28);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:101:11: ( ( 'i' | 'I' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:101:12: ( 'i' | 'I' )
			{
			DebugLocation(101, 12);
			if (input.LA(1)=='I'||input.LA(1)=='i')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("I", 28);
			LeaveRule("I", 28);
			Leave_I();
		}
	}
	// $ANTLR end "I"

	partial void Enter_J();
	partial void Leave_J();

	// $ANTLR start "J"
	[GrammarRule("J")]
	private void mJ()
	{
		Enter_J();
		EnterRule("J", 29);
		TraceIn("J", 29);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:102:11: ( ( 'j' | 'J' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:102:12: ( 'j' | 'J' )
			{
			DebugLocation(102, 12);
			if (input.LA(1)=='J'||input.LA(1)=='j')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("J", 29);
			LeaveRule("J", 29);
			Leave_J();
		}
	}
	// $ANTLR end "J"

	partial void Enter_K();
	partial void Leave_K();

	// $ANTLR start "K"
	[GrammarRule("K")]
	private void mK()
	{
		Enter_K();
		EnterRule("K", 30);
		TraceIn("K", 30);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:103:11: ( ( 'k' | 'K' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:103:12: ( 'k' | 'K' )
			{
			DebugLocation(103, 12);
			if (input.LA(1)=='K'||input.LA(1)=='k')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("K", 30);
			LeaveRule("K", 30);
			Leave_K();
		}
	}
	// $ANTLR end "K"

	partial void Enter_L();
	partial void Leave_L();

	// $ANTLR start "L"
	[GrammarRule("L")]
	private void mL()
	{
		Enter_L();
		EnterRule("L", 31);
		TraceIn("L", 31);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:104:11: ( ( 'l' | 'L' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:104:12: ( 'l' | 'L' )
			{
			DebugLocation(104, 12);
			if (input.LA(1)=='L'||input.LA(1)=='l')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("L", 31);
			LeaveRule("L", 31);
			Leave_L();
		}
	}
	// $ANTLR end "L"

	partial void Enter_M();
	partial void Leave_M();

	// $ANTLR start "M"
	[GrammarRule("M")]
	private void mM()
	{
		Enter_M();
		EnterRule("M", 32);
		TraceIn("M", 32);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:105:11: ( ( 'm' | 'M' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:105:12: ( 'm' | 'M' )
			{
			DebugLocation(105, 12);
			if (input.LA(1)=='M'||input.LA(1)=='m')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("M", 32);
			LeaveRule("M", 32);
			Leave_M();
		}
	}
	// $ANTLR end "M"

	partial void Enter_N();
	partial void Leave_N();

	// $ANTLR start "N"
	[GrammarRule("N")]
	private void mN()
	{
		Enter_N();
		EnterRule("N", 33);
		TraceIn("N", 33);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:106:11: ( ( 'n' | 'N' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:106:12: ( 'n' | 'N' )
			{
			DebugLocation(106, 12);
			if (input.LA(1)=='N'||input.LA(1)=='n')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("N", 33);
			LeaveRule("N", 33);
			Leave_N();
		}
	}
	// $ANTLR end "N"

	partial void Enter_O();
	partial void Leave_O();

	// $ANTLR start "O"
	[GrammarRule("O")]
	private void mO()
	{
		Enter_O();
		EnterRule("O", 34);
		TraceIn("O", 34);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:107:11: ( ( 'o' | 'O' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:107:12: ( 'o' | 'O' )
			{
			DebugLocation(107, 12);
			if (input.LA(1)=='O'||input.LA(1)=='o')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("O", 34);
			LeaveRule("O", 34);
			Leave_O();
		}
	}
	// $ANTLR end "O"

	partial void Enter_P();
	partial void Leave_P();

	// $ANTLR start "P"
	[GrammarRule("P")]
	private void mP()
	{
		Enter_P();
		EnterRule("P", 35);
		TraceIn("P", 35);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:108:11: ( ( 'p' | 'P' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:108:12: ( 'p' | 'P' )
			{
			DebugLocation(108, 12);
			if (input.LA(1)=='P'||input.LA(1)=='p')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("P", 35);
			LeaveRule("P", 35);
			Leave_P();
		}
	}
	// $ANTLR end "P"

	partial void Enter_Q();
	partial void Leave_Q();

	// $ANTLR start "Q"
	[GrammarRule("Q")]
	private void mQ()
	{
		Enter_Q();
		EnterRule("Q", 36);
		TraceIn("Q", 36);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:109:11: ( ( 'q' | 'Q' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:109:12: ( 'q' | 'Q' )
			{
			DebugLocation(109, 12);
			if (input.LA(1)=='Q'||input.LA(1)=='q')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("Q", 36);
			LeaveRule("Q", 36);
			Leave_Q();
		}
	}
	// $ANTLR end "Q"

	partial void Enter_R();
	partial void Leave_R();

	// $ANTLR start "R"
	[GrammarRule("R")]
	private void mR()
	{
		Enter_R();
		EnterRule("R", 37);
		TraceIn("R", 37);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:110:11: ( ( 'r' | 'R' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:110:12: ( 'r' | 'R' )
			{
			DebugLocation(110, 12);
			if (input.LA(1)=='R'||input.LA(1)=='r')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("R", 37);
			LeaveRule("R", 37);
			Leave_R();
		}
	}
	// $ANTLR end "R"

	partial void Enter_S();
	partial void Leave_S();

	// $ANTLR start "S"
	[GrammarRule("S")]
	private void mS()
	{
		Enter_S();
		EnterRule("S", 38);
		TraceIn("S", 38);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:111:11: ( ( 's' | 'S' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:111:12: ( 's' | 'S' )
			{
			DebugLocation(111, 12);
			if (input.LA(1)=='S'||input.LA(1)=='s')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("S", 38);
			LeaveRule("S", 38);
			Leave_S();
		}
	}
	// $ANTLR end "S"

	partial void Enter_T();
	partial void Leave_T();

	// $ANTLR start "T"
	[GrammarRule("T")]
	private void mT()
	{
		Enter_T();
		EnterRule("T", 39);
		TraceIn("T", 39);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:112:11: ( ( 't' | 'T' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:112:12: ( 't' | 'T' )
			{
			DebugLocation(112, 12);
			if (input.LA(1)=='T'||input.LA(1)=='t')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("T", 39);
			LeaveRule("T", 39);
			Leave_T();
		}
	}
	// $ANTLR end "T"

	partial void Enter_U();
	partial void Leave_U();

	// $ANTLR start "U"
	[GrammarRule("U")]
	private void mU()
	{
		Enter_U();
		EnterRule("U", 40);
		TraceIn("U", 40);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:113:11: ( ( 'u' | 'U' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:113:12: ( 'u' | 'U' )
			{
			DebugLocation(113, 12);
			if (input.LA(1)=='U'||input.LA(1)=='u')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("U", 40);
			LeaveRule("U", 40);
			Leave_U();
		}
	}
	// $ANTLR end "U"

	partial void Enter_V();
	partial void Leave_V();

	// $ANTLR start "V"
	[GrammarRule("V")]
	private void mV()
	{
		Enter_V();
		EnterRule("V", 41);
		TraceIn("V", 41);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:114:11: ( ( 'v' | 'V' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:114:12: ( 'v' | 'V' )
			{
			DebugLocation(114, 12);
			if (input.LA(1)=='V'||input.LA(1)=='v')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("V", 41);
			LeaveRule("V", 41);
			Leave_V();
		}
	}
	// $ANTLR end "V"

	partial void Enter_W();
	partial void Leave_W();

	// $ANTLR start "W"
	[GrammarRule("W")]
	private void mW()
	{
		Enter_W();
		EnterRule("W", 42);
		TraceIn("W", 42);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:115:11: ( ( 'w' | 'W' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:115:12: ( 'w' | 'W' )
			{
			DebugLocation(115, 12);
			if (input.LA(1)=='W'||input.LA(1)=='w')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("W", 42);
			LeaveRule("W", 42);
			Leave_W();
		}
	}
	// $ANTLR end "W"

	partial void Enter_X();
	partial void Leave_X();

	// $ANTLR start "X"
	[GrammarRule("X")]
	private void mX()
	{
		Enter_X();
		EnterRule("X", 43);
		TraceIn("X", 43);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:116:11: ( ( 'x' | 'X' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:116:12: ( 'x' | 'X' )
			{
			DebugLocation(116, 12);
			if (input.LA(1)=='X'||input.LA(1)=='x')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("X", 43);
			LeaveRule("X", 43);
			Leave_X();
		}
	}
	// $ANTLR end "X"

	partial void Enter_Y();
	partial void Leave_Y();

	// $ANTLR start "Y"
	[GrammarRule("Y")]
	private void mY()
	{
		Enter_Y();
		EnterRule("Y", 44);
		TraceIn("Y", 44);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:117:11: ( ( 'y' | 'Y' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:117:12: ( 'y' | 'Y' )
			{
			DebugLocation(117, 12);
			if (input.LA(1)=='Y'||input.LA(1)=='y')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("Y", 44);
			LeaveRule("Y", 44);
			Leave_Y();
		}
	}
	// $ANTLR end "Y"

	partial void Enter_Z();
	partial void Leave_Z();

	// $ANTLR start "Z"
	[GrammarRule("Z")]
	private void mZ()
	{
		Enter_Z();
		EnterRule("Z", 45);
		TraceIn("Z", 45);
		try
		{
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:118:11: ( ( 'z' | 'Z' ) )
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:118:12: ( 'z' | 'Z' )
			{
			DebugLocation(118, 12);
			if (input.LA(1)=='Z'||input.LA(1)=='z')
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
			TraceOut("Z", 45);
			LeaveRule("Z", 45);
			Leave_Z();
		}
	}
	// $ANTLR end "Z"

	public override void mTokens()
	{
		// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:8: ( L_CURL_BRACKET | R_CURL_BRACKET | L_SQUARE_BRACKET | R_SQUARE_BRACKET | SEMICOLON | EQUALS | COMMA | QUOTED_VALUE | SUBGRAPH | GRAPH | NODE | EDGE | EDGE_OPERATOR_NOARROW | EDGE_OPERATOR_ARROW | ALLOWED_QUOTED_VALUES | ID | WS )
		int alt6=17;
		try { DebugEnterDecision(6, decisionCanBacktrack[6]);
		try
		{
			alt6 = dfa6.Predict(input);
		}
		catch (NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
			throw;
		}
		} finally { DebugExitDecision(6); }
		switch (alt6)
		{
		case 1:
			DebugEnterAlt(1);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:10: L_CURL_BRACKET
			{
			DebugLocation(1, 10);
			mL_CURL_BRACKET(); 

			}
			break;
		case 2:
			DebugEnterAlt(2);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:25: R_CURL_BRACKET
			{
			DebugLocation(1, 25);
			mR_CURL_BRACKET(); 

			}
			break;
		case 3:
			DebugEnterAlt(3);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:40: L_SQUARE_BRACKET
			{
			DebugLocation(1, 40);
			mL_SQUARE_BRACKET(); 

			}
			break;
		case 4:
			DebugEnterAlt(4);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:57: R_SQUARE_BRACKET
			{
			DebugLocation(1, 57);
			mR_SQUARE_BRACKET(); 

			}
			break;
		case 5:
			DebugEnterAlt(5);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:74: SEMICOLON
			{
			DebugLocation(1, 74);
			mSEMICOLON(); 

			}
			break;
		case 6:
			DebugEnterAlt(6);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:84: EQUALS
			{
			DebugLocation(1, 84);
			mEQUALS(); 

			}
			break;
		case 7:
			DebugEnterAlt(7);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:91: COMMA
			{
			DebugLocation(1, 91);
			mCOMMA(); 

			}
			break;
		case 8:
			DebugEnterAlt(8);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:97: QUOTED_VALUE
			{
			DebugLocation(1, 97);
			mQUOTED_VALUE(); 

			}
			break;
		case 9:
			DebugEnterAlt(9);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:110: SUBGRAPH
			{
			DebugLocation(1, 110);
			mSUBGRAPH(); 

			}
			break;
		case 10:
			DebugEnterAlt(10);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:119: GRAPH
			{
			DebugLocation(1, 119);
			mGRAPH(); 

			}
			break;
		case 11:
			DebugEnterAlt(11);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:125: NODE
			{
			DebugLocation(1, 125);
			mNODE(); 

			}
			break;
		case 12:
			DebugEnterAlt(12);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:130: EDGE
			{
			DebugLocation(1, 130);
			mEDGE(); 

			}
			break;
		case 13:
			DebugEnterAlt(13);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:135: EDGE_OPERATOR_NOARROW
			{
			DebugLocation(1, 135);
			mEDGE_OPERATOR_NOARROW(); 

			}
			break;
		case 14:
			DebugEnterAlt(14);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:157: EDGE_OPERATOR_ARROW
			{
			DebugLocation(1, 157);
			mEDGE_OPERATOR_ARROW(); 

			}
			break;
		case 15:
			DebugEnterAlt(15);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:177: ALLOWED_QUOTED_VALUES
			{
			DebugLocation(1, 177);
			mALLOWED_QUOTED_VALUES(); 

			}
			break;
		case 16:
			DebugEnterAlt(16);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:199: ID
			{
			DebugLocation(1, 199);
			mID(); 

			}
			break;
		case 17:
			DebugEnterAlt(17);
			// D:\\Dokumenty\\Steve\\Dokumenty\\Programovani\\Vlastni-projekty\\Graphviz4Net\\cur-codeplex\\src\\Graphviz4Net.Core\\Dot\\AntlrParser\\DotGrammar.g:1:202: WS
			{
			DebugLocation(1, 202);
			mWS(); 

			}
			break;

		}

	}


	#region DFA
	DFA6 dfa6;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa6 = new DFA6(this);
	}

	private class DFA6 : DFA
	{
		private const string DFA6_eotS =
			"\x9\xFFFF\x5\x10\x4\xFFFF\x5\x10\x2\xFFFF\x8\x10\x1\x26\x1\x27\x1\x10"+
			"\x1\x29\x1\x10\x2\xFFFF\x1\x10\x1\xFFFF\x2\x10\x1\x29\x1\x2E\x1\xFFFF";
		private const string DFA6_eofS =
			"\x2F\xFFFF";
		private const string DFA6_minS =
			"\x1\x9\x8\xFFFF\x1\x55\x1\x52\x1\x49\x1\x4F\x1\x44\x1\x2D\x3\xFFFF\x1"+
			"\x42\x1\x41\x1\x47\x1\x44\x1\x47\x2\xFFFF\x1\x47\x1\x50\x1\x52\x2\x45"+
			"\x1\x52\x1\x48\x1\x41\x2\x30\x1\x41\x1\x30\x1\x50\x2\xFFFF\x1\x50\x1"+
			"\xFFFF\x2\x48\x2\x30\x1\xFFFF";
		private const string DFA6_maxS =
			"\x1\x7D\x8\xFFFF\x1\x75\x1\x72\x1\x69\x1\x6F\x1\x64\x1\x3E\x3\xFFFF"+
			"\x1\x62\x1\x61\x1\x67\x1\x64\x1\x67\x2\xFFFF\x1\x67\x1\x70\x1\x72\x2"+
			"\x65\x1\x72\x1\x68\x1\x61\x2\x7A\x1\x61\x1\x7A\x1\x70\x2\xFFFF\x1\x70"+
			"\x1\xFFFF\x2\x68\x2\x7A\x1\xFFFF";
		private const string DFA6_acceptS =
			"\x1\xFFFF\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x6\xFFFF\x1"+
			"\xF\x1\x10\x1\x11\x5\xFFFF\x1\xD\x1\xE\xD\xFFFF\x1\xB\x1\xC\x1\xFFFF"+
			"\x1\xA\x4\xFFFF\x1\x9";
		private const string DFA6_specialS =
			"\x2F\xFFFF}>";
		private static readonly string[] DFA6_transitionS =
			{
				"\x2\x11\x2\xFFFF\x1\x11\x12\xFFFF\x1\x11\x1\xFFFF\x1\x8\x9\xFFFF\x1"+
				"\x7\x1\xE\x1\xF\x1\xFFFF\xB\x10\x1\x5\x1\xFFFF\x1\x6\x3\xFFFF\x3\x10"+
				"\x1\xB\x1\xD\x1\x10\x1\xA\x6\x10\x1\xC\x4\x10\x1\x9\x7\x10\x1\x3\x1"+
				"\xFFFF\x1\x4\x1\xFFFF\x1\x10\x1\xFFFF\x3\x10\x1\xB\x1\xD\x1\x10\x1\xA"+
				"\x6\x10\x1\xC\x4\x10\x1\x9\x7\x10\x1\x1\x1\xFFFF\x1\x2",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\x12\x1F\xFFFF\x1\x12",
				"\x1\x13\x1F\xFFFF\x1\x13",
				"\x1\x14\x1F\xFFFF\x1\x14",
				"\x1\x15\x1F\xFFFF\x1\x15",
				"\x1\x16\x1F\xFFFF\x1\x16",
				"\x1\x17\x10\xFFFF\x1\x18",
				"",
				"",
				"",
				"\x1\x19\x1F\xFFFF\x1\x19",
				"\x1\x1A\x1F\xFFFF\x1\x1A",
				"\x1\x1B\x1F\xFFFF\x1\x1B",
				"\x1\x1C\x1F\xFFFF\x1\x1C",
				"\x1\x1D\x1F\xFFFF\x1\x1D",
				"",
				"",
				"\x1\x1E\x1F\xFFFF\x1\x1E",
				"\x1\x1F\x1F\xFFFF\x1\x1F",
				"\x1\x20\x1F\xFFFF\x1\x20",
				"\x1\x21\x1F\xFFFF\x1\x21",
				"\x1\x22\x1F\xFFFF\x1\x22",
				"\x1\x23\x1F\xFFFF\x1\x23",
				"\x1\x24\x1F\xFFFF\x1\x24",
				"\x1\x25\x1F\xFFFF\x1\x25",
				"\xB\x10\x6\xFFFF\x1A\x10\x4\xFFFF\x1\x10\x1\xFFFF\x1A\x10",
				"\xB\x10\x6\xFFFF\x1A\x10\x4\xFFFF\x1\x10\x1\xFFFF\x1A\x10",
				"\x1\x28\x1F\xFFFF\x1\x28",
				"\xB\x10\x6\xFFFF\x1A\x10\x4\xFFFF\x1\x10\x1\xFFFF\x1A\x10",
				"\x1\x2A\x1F\xFFFF\x1\x2A",
				"",
				"",
				"\x1\x2B\x1F\xFFFF\x1\x2B",
				"",
				"\x1\x2C\x1F\xFFFF\x1\x2C",
				"\x1\x2D\x1F\xFFFF\x1\x2D",
				"\xB\x10\x6\xFFFF\x1A\x10\x4\xFFFF\x1\x10\x1\xFFFF\x1A\x10",
				"\xB\x10\x6\xFFFF\x1A\x10\x4\xFFFF\x1\x10\x1\xFFFF\x1A\x10",
				""
			};

		private static readonly short[] DFA6_eot = DFA.UnpackEncodedString(DFA6_eotS);
		private static readonly short[] DFA6_eof = DFA.UnpackEncodedString(DFA6_eofS);
		private static readonly char[] DFA6_min = DFA.UnpackEncodedStringToUnsignedChars(DFA6_minS);
		private static readonly char[] DFA6_max = DFA.UnpackEncodedStringToUnsignedChars(DFA6_maxS);
		private static readonly short[] DFA6_accept = DFA.UnpackEncodedString(DFA6_acceptS);
		private static readonly short[] DFA6_special = DFA.UnpackEncodedString(DFA6_specialS);
		private static readonly short[][] DFA6_transition;

		static DFA6()
		{
			int numStates = DFA6_transitionS.Length;
			DFA6_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA6_transition[i] = DFA.UnpackEncodedString(DFA6_transitionS[i]);
			}
		}

		public DFA6( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 6;
			this.eot = DFA6_eot;
			this.eof = DFA6_eof;
			this.min = DFA6_min;
			this.max = DFA6_max;
			this.accept = DFA6_accept;
			this.special = DFA6_special;
			this.transition = DFA6_transition;
		}

		public override string Description { get { return "1:1: Tokens : ( L_CURL_BRACKET | R_CURL_BRACKET | L_SQUARE_BRACKET | R_SQUARE_BRACKET | SEMICOLON | EQUALS | COMMA | QUOTED_VALUE | SUBGRAPH | GRAPH | NODE | EDGE | EDGE_OPERATOR_NOARROW | EDGE_OPERATOR_ARROW | ALLOWED_QUOTED_VALUES | ID | WS );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

 
	#endregion

}

} // namespace Graphviz4Net.Dot.AntlrParser

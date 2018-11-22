grammar DotGrammar;

options {
	language=CSharp3;
}

tokens {
	L_CURL_BRACKET = '{';
	R_CURL_BRACKET = '}';
	
	L_SQUARE_BRACKET = '[';	
	R_SQUARE_BRACKET = ']';
	
	SEMICOLON = ';';	
	EQUALS = '=';	
	COMMA = ',';
//	QUOTE = '"';
}



@lexer::namespace {Graphviz4Net.Dot.AntlrParser}
@parser::namespace {Graphviz4Net.Dot.AntlrParser}

@header {
	using Graphviz4Net.Dot;
}

public dot: GRAPH opt_id L_CURL_BRACKET stmt_list R_CURL_BRACKET EOF;

id returns [string id_content]	: token = ID {id_content = token.Text;};
opt_id	: id |;
	
stmt_list 
	: (subgraph_stmt stmt_list) | (stmt SEMICOLON stmt_list) |;
	  
stmt	: 
	GRAPH graph_attrs = opt_attr_list { this.AddGraphAttributes(graph_attrs); } |
	vertex_id = id stmt_opt_attr_list = opt_attr_list { this.AddVertex(vertex_id, stmt_opt_attr_list); } |
	source_id = id (EDGE_OPERATOR_NOARROW|EDGE_OPERATOR_ARROW) dest_id = id edge_attrs = opt_attr_list 
		{ this.AddEdge(source_id, dest_id, edge_attrs); } |
	NODE opt_attr_list |
	EDGE opt_attr_list |;	
	
subgraph_stmt 
	:	SUBGRAPH sub_graph_id = id { this.EnterSubGraph(sub_graph_id); } L_CURL_BRACKET stmt_list R_CURL_BRACKET { this.LeaveSubGraph(); };

opt_attr_list returns [IDictionary<string, string> opt_attr_list_result]
	: L_SQUARE_BRACKET opt_attr_list_result_value = attr_list R_SQUARE_BRACKET { opt_attr_list_result = opt_attr_list_result_value; } |
	  L_SQUARE_BRACKET R_SQUARE_BRACKET { opt_attr_list_result = new Dictionary<string, string>(); } | 
	 { opt_attr_list_result = new Dictionary<string, string>(); };

attr_list returns [IDictionary<string, string> attr_list_result]
	@init { attr_list_result = new Dictionary<string, string>(); }
	: attr_list_attr = attr { attr_list_result.Add(attr_list_attr); }
	  (COMMA attr_list_attr2 = attr { attr_list_result.Add(attr_list_attr2); })*;

attr returns [KeyValuePair<string, string> attr_result]
	: attr_result_id = id EQUALS attr_result_value = attr_value 
	{ attr_result = new KeyValuePair<string, string>(attr_result_id, attr_result_value); };
	
attr_value returns [string attr_value_result]
	: attr_value_result_value = ID { attr_value_result = attr_value_result_value.Text; } |
	  attr_value_quoted = QUOTED_VALUE { attr_value_result = this.Unquote(attr_value_quoted.Text); };
	 

QUOTED_VALUE	:	'"' (~'"')* '"';

// keywords
SUBGRAPH: S U B G R A P H;
GRAPH	: (G R A P H|D I G R A P H);
NODE	: N O D E;
EDGE	: E D G E;
EDGE_OPERATOR_NOARROW :	'--';
EDGE_OPERATOR_ARROW :	'->';

// ids and attr values
ALLOWED_QUOTED_VALUES : ('.')+ ;
ID  :   (STR|NUMBER|'_'|':'|'.')+ ;	/* NOTE: theoretically, we need to allow even '\200'..'\377' */
fragment STR	: ('a'..'z'|'A'..'Z');
fragment NUMBER : '0'..'9';

// whitespace: hidden from parser
WS  :   (' '|'\t'|'\r'|'\n')+ {
#if SOMETHING_THAT_DOES_NOT_EXIST
$channel=HIDDEN;	// antlr works does not recognize C# constant Hidden
#else
$channel=Hidden;
#endif
};

// case insensitive letters:
fragment A:('a'|'A');
fragment B:('b'|'B');
fragment C:('c'|'C');
fragment D:('d'|'D');
fragment E:('e'|'E');
fragment F:('f'|'F');
fragment G:('g'|'G');
fragment H:('h'|'H');
fragment I:('i'|'I');
fragment J:('j'|'J');
fragment K:('k'|'K');
fragment L:('l'|'L');
fragment M:('m'|'M');
fragment N:('n'|'N');
fragment O:('o'|'O');
fragment P:('p'|'P');
fragment Q:('q'|'Q');
fragment R:('r'|'R');
fragment S:('s'|'S');
fragment T:('t'|'T');
fragment U:('u'|'U');
fragment V:('v'|'V');
fragment W:('w'|'W');
fragment X:('x'|'X');
fragment Y:('y'|'Y');
fragment Z:('z'|'Z');


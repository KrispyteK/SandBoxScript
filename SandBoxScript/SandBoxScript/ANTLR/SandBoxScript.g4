grammar SandBoxScript;

program				: block EOF ;

block				locals [
					Dictionary<string, SandBoxScript.Variable> Variables = new Dictionary<string, SandBoxScript.Variable>(),
					]
					: (
					importStmnt
					| ifStmnt
					| fnStmnt 
					| returnStmnt
					| assignStmnt
					| expression
					)*
					;

stmntBlock			: '{' Block=block '}'
					| returnStmnt
					| assignStmnt
					| expression
					;

importStmnt			: IMPORT Target=expression																					#importStatement
					;

fnStmnt				locals [
					Dictionary<string, SandBoxScript.Variable> ParameterVariables = new Dictionary<string, SandBoxScript.Variable>(),
					BaseValue ReturnValue = null
					]
					: FN name '(' parameterGroup ')' {
var fnCtx = ($ctx as FunctionStatementContext);
var nameCtx = fnCtx.name();

var newVar = new SandBoxScript.Variable(nameCtx.GetText());

$block::Variables[nameCtx.GetText()] = newVar;
nameCtx.variable = newVar;		

var parameters = fnCtx.parameterGroup().parameter();
var processedParameters = new SandBoxScript.Parameter[parameters.Length];

for (var i = 0; i < parameters.Length; i++) {
	var p = parameters[i];
	var name = p.NAME().GetText();

	processedParameters[i] = new SandBoxScript.Parameter(name, p.expression()); 

	var parameterVar = new SandBoxScript.Variable(name);

	$fnStmnt::ParameterVariables[name] = parameterVar;
}

} stmntBlock {
	var function = new SandBoxScript.ScriptFunction(fnCtx) { 
		Parameters = processedParameters
	}; 
	var functionVar = new SandBoxScript.FunctionInstance(this.Engine, function); 

	newVar.Value = functionVar;													
}																																#functionStatement
					;											

returnStmnt			locals [
					FunctionStatementContext Statement
					]
					: RETURN expression? {
RuleContext currentContext = $ctx;
RuleContext functionStatementCtx = null;

while (currentContext.Parent != null) {
	if (currentContext is FunctionStatementContext fnCtx) {
		functionStatementCtx = currentContext;
		$Statement = fnCtx;
		break;
	}

	currentContext = currentContext.Parent;
}	

if (functionStatementCtx == null) {
	throw new RecognitionException("Return statement must be inside a function", this, this._input, $ctx);
}
}																																#returnStatement
					;

parameterGroup		: (parameter (',' parameter)*)? ;							

parameter			: NAME ('=' expression)?;

ifStmnt				: if (elseif)* else?																						#ifStatement			
					;

if					: IF '(' Condition=expression ')' stmntBlock 
					;

elseif				: ELSE IF '(' Condition=expression ')' stmntBlock
					;

else				: ELSE stmntBlock																			
					;

assignStmnt			: name	{
var nameCtx = ($ctx as AssignNameStatementContext).name();

if (nameCtx.variable == null) {
	var newVar = new SandBoxScript.Variable(nameCtx.GetText());

	$block::Variables[nameCtx.GetText()] = newVar;
	nameCtx.variable = newVar;
}
}			
																		ASSIGN expression										#assignNameStatement
					| memberAccess										ASSIGN expression										#assignMemberStatement
					| memberAccessComp									ASSIGN expression										#assignComputedMemberStatement					
					;

expression          : '(' expression ')'																						#parenthesisExp
					| expression DOT NAME																						#memberAccessExp
					| expression '[' expression ']'																				#computedMemberAccessExp
                    | Function=expression '(' Arguments=expressionGroup ')'														#functionCallExp
					
					| <assoc=right>		Left=expression Operation=EXPONENT			Right=expression							#binaryOperationExp
                    |					Left=expression Operation=(ASTERISK|SLASH)	Right=expression							#binaryOperationExp
                    |					Left=expression Operation=(PLUS|MINUS)		Right=expression							#binaryOperationExp

                    |					Left=expression Operation=(LESS|LESSEQ|GREATER|GREATEREQ)	Right=expression			#binaryOperationExp
                    |					Left=expression Operation=(EQUAL|NOTEQUAL)					Right=expression			#binaryOperationExp

					| name {
var nameCtx = ($ctx as NameExpContext).name();

if (nameCtx.variable == null) {
	throw new RecognitionException("Undefined variable: " + nameCtx.GetText(), this, this._input, $ctx);
}																								
}																																#nameExp
                    | number																									#numberLiteral
					| string																									#stringLiteral
					| vector2																									#vector2Literal
					| vector3																									#vector3Literal
					| vector4																									#vector4Literal
                    ;

name returns [SandBoxScript.Variable variable] : NAME 	{
RuleContext currentContext = $ctx;

while (currentContext.Parent != null) {

	if (currentContext is BlockContext blockCtx) {
		if (blockCtx.Variables.ContainsKey($NAME.text)) {
			$variable = blockCtx.Variables[$NAME.text];
			break;
		}
	}

	if (currentContext is FunctionStatementContext fnCtx) {		
		if (fnCtx.ParameterVariables.ContainsKey($NAME.text)) {
			$variable = fnCtx.ParameterVariables[$NAME.text];
			break;
		}
	}

	currentContext = currentContext.Parent;
}

if ($variable == null && this.Globals.ContainsKey($NAME.text)) {
	$variable = this.Globals[$NAME.text];
}
} ;

memberAccess		: expression DOT NAME ;
memberAccessComp	: expression '[' expression ']' ;

string returns [string value] : STRING { 
var content = $STRING.text.Substring(1,  $STRING.text.Length - 2);

$value = System.Text.RegularExpressions.Regex.Unescape(content);
} ;

number returns [double value] : NUMBER { 
$value = double.Parse($NUMBER.text); 
} ;

vector2				:	'<' X=expression ',' Y=expression '>' ;
vector3				:	'<' X=expression ',' Y=expression ',' Z=expression '>' ;
vector4				:	'<' X=expression ',' Y=expression ',' Z=expression ',' W=expression'>' ;

expressionGroup		: (expression (',' expression)*)? ;

fragment LETTER			: [a-zA-Z] ;
fragment DIGIT			: [0-9] ;
fragment ESCAPED_QUOTE	: '\\"';

DOT						: '.' ;

IMPORT					: 'import' ;
IF						: 'if' ;
ELSE					: 'else' ;
FN						: 'fn' ;
RETURN					: 'return' ;
BREAK					: 'break' ;
CONTINUE				: 'continue' ;

KEYWORD					: (IMPORT | IF | ELSE | FN | RETURN | BREAK | CONTINUE) ;

STRING : '"' ~('"')* ('"' | {throw new Antlr4.Runtime.Misc.ParseCanceledException("Unterminated string detected");}) ;

LESS				: '<'	;
LESSEQ				: '<='	;
GREATER				: '>'	;
GREATEREQ			: '>='	;
EQUAL				: '=='	;
NOTEQUAL			: '!='	;

ASSIGN	            : '='	;

ASTERISK            : '*'	;
SLASH               : '/'	;
PLUS                : '+'	;
MINUS               : '-'	;
EXPONENT            : '**'	;

INCREMENT			: '++'	;
DECREMENT			: '--'	;

NAME				: LETTER (LETTER | DIGIT)*;

NUMBER              : DIGIT+ ('.' DIGIT+)?;

WHITESPACE : [ \n\t\r]+ -> channel(HIDDEN);

COMMENT : '//' .* '\n' -> channel(HIDDEN);
COMMENTMULTILINE : '/*' .* '*/' -> channel(HIDDEN);

// handle characters which failed to match any other token
ErrorCharacter : . ;
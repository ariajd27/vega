grammar Requisites;

file : requisite (DIVIDER? requisite)* DIVIDER? EOF;

requisite
    : course_requisite
    | grade_requisite
    | plan_requisite
    ;
course_requisite
    : requisite_type COLON waffle? subjectless_expression
    ;
subjectless_expression 
    : subjectless_expression (COMMA subjectless_expression)* COMMA? binary_operator subjectless_expression 
    | LPAREN subjectless_expression RPAREN 
    | catalog_subject subject_expression
    | catalog_range
    | subjectless_expression sub_grade_requisite
    | subjectless_expression subjectless_expression
    | waffle COLON subjectless_expression
    ;
subjectless_exception
    : EXCEPT subjectless_expression
    ;
subject_expression
    : subject_expression (COMMA subject_expression)* COMMA? binary_operator subject_expression 
    | LPAREN subject_expression RPAREN 
    | catalog_number
    | subject_expression sub_grade_requisite
    ; 
grade_requisite
    : SEMI? GRADE_MIN COLON? QUOTE? letter_grade QUOTE? grade_requisite_domain?
    | LPAREN GRADE_MIN COLON? QUOTE? letter_grade QUOTE? grade_requisite_domain? RPAREN?
    | LPAREN waffle? letter_grade OR_HIGHER RPAREN
    ;
sub_grade_requisite
    : LPAREN GRADE_MIN COLON? QUOTE? letter_grade QUOTE? RPAREN
    | LPAREN waffle? letter_grade OR_HIGHER RPAREN
    ;
grade_requisite_domain
    : FOR? subjectless_expression
    | FOR? ALL_LISTED_COURSES subjectless_exception?
    ;
plan_requisite
    : PLAN COLON waffle_expression
    ;

waffle_expression
    : waffle_expression binary_operator waffle_expression
    | LPAREN waffle_expression RPAREN
    | waffle degree_expression?
    ;
degree_expression
    : degree_expression binary_operator degree_expression
    | degree_expression COMMA degree_expression
    | LPAREN degree_expression RPAREN
    | degree
    ;
    
waffle : (WAFFLE | ALPHA_CAPS | ALPHA_PASCAL)+ ;
catalog_range : catalog_subject RANGE_STRING catalog_subject? catalog_number ;
requisite_type : REQUISITE_LABEL ;
letter_grade : LETTER_GRADE ;
degree : DEGREE_NAME ;
catalog_subject : ALPHA_CAPS | ALPHA_PASCAL ;
catalog_number : NUMBER ;
binary_operator : AND | OR ;

OR_HIGHER 
    : 'or higher' 
    | 'or better'
    | 'or Better'
    ;
    
REQUISITE_LABEL : PREQ | CREQ ;
DEGREE_NAME : DEGREE ;

WS : [ \t]+ -> channel(HIDDEN) ;
DIVIDER
    : SEMI
    | PERIOD
    ;
    
AND 
    : '&' 
    | [Aa][Nn][Dd]
    ;
OR 
    : '/'
    | [Oo][Rr]
    ;
PREQ 
    : [Pp](('RE'|'re')'-'?)+[Qq]('UISITE'|'uisite')?[Ss]?
    ;
CREQ 
    : [Cc][Oo]?'-'?('REQ'|'req')('UISITE'|'uisite')?[Ss]?
    ;
GRADE_MIN 
    : ('MIN'|[Mm]'in')?' '('GRAD'|[Gg]'rad')[Ee]?
    ;
PLAN
    : [Pp]('LAN'|'lan'|'ATH'|'ath'|'ROG'|'rog')
    ;
    
RANGE_STRING 
    : 'greater than or equal to' 
    ;
ALL_LISTED_COURSES 
    : ALL(' 'LISTED)?' 'COURSES?(' 'LISTED)?
    ;
ALL
    : [Aa]('LL'|'ll')
    ;
LISTED
    : [Ll]('ISTED'|'isted')
    ;
COURSES
    : [Cc]('OURSES'|'our''s'?'es')
    | [Cc]('LASSES'|'lasses')
    ;
EXCEPT 
    : 'except' 
    ;
FOR
    : [Ff][Oo][Rr]
    ;

DEGREE
    : 'BS'
    | 'BA'
    | 'BP'[Hh][Ii]?[Ll]?
    | 'MS'
    | 'MA'
    | 'MD'
    | 'P'[Hh]'D'
    ;

SEMI : ';' ;
COLON : ':' ;
COMMA : ',' ;
PERIOD : '.' ;
LPAREN : '(' | '[' | '{' ;
RPAREN : ')' | ']' | '}' ;
QUOTE : '\'' | '"' ;

NUMBER : [0-9]+ ;
LETTER_GRADE : [A-Z][+-]? ;
ALPHA_CAPS : [A-Z][A-Z]+ ;
ALPHA_PASCAL : [A-Z][a-z]+ ;
WAFFLE : [a-zA-Z_\-]+ ;
.data

;tworzê sobie sta³a 255 do dzielenia
 const dd 255

.code

;wyliczam odpowiednie wartoœci dla koloru niebieskiego, zielonego i czerwone
MyProc1 proc

; ze stacka do mm0 aktualny iterator pêtli
movq mm0,[rsp+40]

; przeniesienie iteratora do r13
movd r13,mm0

; przemnozenie iteratora przez 4
imul r13,4

;konwercja byte do dworda
PMOVZXBD xmm4,[rcx+r13] 
PMOVZXBD xmm5,[rcx+r13+1] 
PMOVZXBD xmm6,[rcx+r13+2] 

;konwercja dworda do floata
CVTDQ2PS xmm4,xmm4
CVTDQ2PS xmm5,xmm5
CVTDQ2PS xmm6,xmm6

;stala 255 do xmm7 
MOVups xmm7, [const]

;konwersja sta³ej na floata
CVTDQ2PS xmm7,xmm7

;podzielenie danego koloru przez sta³¹ 255
DIVPS xmm1,xmm8
DIVPS xmm2,xmm8
DIVPS xmm3,xmm8

;mno¿enie odpowiedniej wartoœci z tablicy bajtów z ka¿dym kolorem
MULPS xmm1,xmm4;
MULPS xmm2,xmm5;
MULPS xmm3,xmm6;

;przeniesienie aktualnego indeksu do rejestru mm1
movd mm1,r13

;przeniesienie wartosci zmiennoprzecinkowej koloru niebieskiego do rejestru xmm0
movaps xmm0, xmm1

;zwrot wartosci w rejestrze xmm0
ret

MyProc1 endp

;procedura 2 zwraca wartosc koloru zielonego
.code
MyProc2 proc
movaps xmm0, xmm2
ret
MyProc2 endp

;procedura 3 zwraca wartosc koloru zielonego
.code
MyProc3 proc
movaps xmm0, xmm3
ret
MyProc3 endp

;procedura 4 zwraca aktualny indeks tablicy bajtów
.code
MyProc4 proc
movd rax,mm1
ret
MyProc4 endp
end
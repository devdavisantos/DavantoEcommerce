export default function Footer() {
    return (
        <footer className="bg-[#131921] text-white mt-16">
          <div className="mx-auto max-w-6xl px-4 py-6"> {/* diminuiu de py-12 para py-6 */}

            <div className="grid grid-cols-1 gap-4 md:grid-cols-3"> {/* gap menor */}
              <div>
                <h3 className="text-base font-bold mb-2">AutoStore</h3> {/* diminuí fonte e margem */}
                <p className="text-xs text-gray-300"> {/* fonte menor */}
                  Seu portal automotivo completo. Em breve com anúncios de veículos, peças e serviços.
                </p>
              </div>

              <div>
                <h3 className="text-base font-bold mb-2">Contato</h3>
                <ul className="space-y-1 text-xs text-gray-300"> {/* menos space - fonte menor */}
                  <li>(11) 4002-8922</li>
                  <li>contato@autostore.com.br</li>
                  <li>São Paulo, SP</li>
                </ul>
              </div>

              <div>
                <h3 className="text-base font-bold mb-2">Horário de Atendimento</h3>
                <ul className="space-y-1 text-xs text-gray-300">
                  <li>Segunda - Sexta: 8h às 18h</li>
                  <li>Sábado: 9h às 13h</li>
                  <li>Domingo: Fechado</li>
                </ul>
              </div>
            </div>

            {/* Área de direitos autorais  — permanece igual */}
            <div className="w-full max-h-0.5 mt-8 border-t border-gray-700 p-3 text-center text-sm text-gray-400">
              <p>© {new Date().getFullYear()} LAStore. Todos os direitos reservados.</p>
            </div>
          </div>
        </footer>
    );
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RulesEngine.Models;
using RulesEngine.Extensions;
using System.Text.Json;

namespace WorkflowEngine.RuleEngine.Engines
{
    public class RuleEvaluator
    {
        public async Task<bool> EvaluateAsync(string expression, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(expression))
                return true;

            // Microsoft RulesEngine requires a list of Workflow objects
            var rules = new List<Rule>
            {
                new Rule
                {
                    RuleName = "DynamicRule",
                    Expression = expression,
                    Enabled = true
                }
            };

            var workflow = new List<Workflow>
            {
                new Workflow
                {
                    WorkflowName = "DynamicWorkflow",
                    Rules = rules
                }
            };

            var engine = new RulesEngine.RulesEngine(workflow.ToArray());

            // Convert dictionary to dynamic inputs, unwrapping JsonElement
            var paramDictionary = parameters ?? new Dictionary<string, object>();
            var ruleParams = new List<RuleParameter>();

            foreach (var p in paramDictionary)
            {
                object finalValue = p.Value;
                if (finalValue is JsonElement jsonElement)
                {
                    switch (jsonElement.ValueKind)
                    {
                        case JsonValueKind.Number:
                            if (jsonElement.TryGetInt64(out long l)) finalValue = l;
                            else if (jsonElement.TryGetDouble(out double d)) finalValue = d;
                            break;
                        case JsonValueKind.True:
                            finalValue = true;
                            break;
                        case JsonValueKind.False:
                            finalValue = false;
                            break;
                        case JsonValueKind.String:
                        default:
                            finalValue = jsonElement.GetString();
                            // Attempt to parse string to double if it represents a number
                            if (double.TryParse(finalValue?.ToString(), out double parsedNum))
                            {
                                finalValue = parsedNum;
                            }
                            break;
                    }
                }
                ruleParams.Add(new RuleParameter(p.Key, finalValue));
            }

            var results = await engine.ExecuteAllRulesAsync("DynamicWorkflow", ruleParams.ToArray());

            // Check if our rule was successful
            return results.Any(r => r.IsSuccess);
        }
    }
}
